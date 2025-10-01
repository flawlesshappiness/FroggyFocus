using Godot;
using System.Collections;
using System.Collections.Generic;

public partial class SkillCheckProjectile : Node3D
{
    [Export]
    public float Radius;

    [Export]
    public Vector2 SpeedMin;

    [Export]
    public Vector2 SpeedMax;

    [Export]
    public EasingFunctions.Ease SpeedCurve = EasingFunctions.Ease.EaseInQuad;

    [Export]
    public MoveType Movement;

    [Export]
    public float Damage;

    [Export]
    public bool IsPercentage = true;

    [Export]
    public ParticleEffectGroup PsIdle;

    [Export]
    public ParticleEffectGroup PsHurt;

    public bool IsHit { get; private set; }
    protected float DistanceToCursor => GlobalPosition.DistanceTo(settings.FocusEvent.Cursor.GlobalPosition);

    private float time_start;
    private Settings settings;

    public enum MoveType
    {
        Linear,
        Glitch,
    };

    public class Settings
    {
        public FocusEvent FocusEvent { get; set; }
        public FocusCursor Cursor => FocusEvent.Cursor;
        public FocusCursorShield Shield => Cursor.Shield;
        public FocusTarget Target => FocusEvent.Target;
        public List<Vector3> HitPositions { get; private set; } = new();
    }

    public Coroutine StartProjectile(Settings settings)
    {
        this.settings = settings;
        time_start = GameTime.Time;
        return this.StartCoroutine(Cr, "projectile");

        IEnumerator Cr()
        {
            yield return WaitForMoveTowardsPlayer();
            yield return new WaitForSeconds(0.05f); // grace period
            ValidatePlayerDistance(true);
            IsHit = true;
        }
    }

    public IEnumerator WaitForMoveTowardsPosition(Vector3 position)
    {
        var dist = GlobalPosition.DistanceTo(position);
        while (dist > Radius)
        {
            var velocity = GetNextVelocity(position);
            GlobalPosition += velocity;
            ValidatePlayerDistance(false);
            yield return WaitForFrequency();
            dist = GlobalPosition.DistanceTo(position);
        }
    }

    public IEnumerator WaitForMoveTowardsPlayer()
    {
        var dist = GlobalPosition.DistanceTo(settings.Cursor.GlobalPosition);
        while (dist > Radius)
        {
            var velocity = GetNextVelocity(settings.Cursor.GlobalPosition);
            GlobalPosition += velocity;
            ValidatePlayerDistance(false);
            yield return WaitForFrequency();
            dist = GlobalPosition.DistanceTo(settings.Cursor.GlobalPosition);
        }
    }

    private IEnumerator WaitForFrequency()
    {
        yield return Movement == MoveType.Glitch ? new WaitForSeconds(0.15f) : null;
    }

    private Vector3 GetNextVelocity(Vector3 position)
    {
        var dir = position - GlobalPosition;
        var speed = GetNextSpeed();
        var velocity = dir.Normalized() * speed;
        var clamped = velocity.ClampMagnitude(0, dir.Length());
        return clamped;
    }

    private float GetNextSpeed()
    {
        var mul = Movement == MoveType.Glitch ? 1.0f : GameTime.DeltaTime;
        var min = SpeedMin.Range(settings.FocusEvent.Target.Difficulty);
        var max = SpeedMax.Range(settings.FocusEvent.Target.Difficulty);
        var t_time = (GameTime.Time - time_start) / 0.5f;
        var curve = Curves.GetCurve(SpeedCurve);
        var t = curve.Evaluate(t_time);
        return Mathf.Lerp(min, max, t) * mul;
    }

    private void ValidatePlayerDistance(bool can_hurt)
    {
        if (IsHit) return;

        var shield_radius = Radius + settings.Cursor.Radius;
        var near_shield = DistanceToCursor < shield_radius;
        var is_shielded = settings.Shield.IsShielded;

        if (near_shield && (can_hurt || is_shielded))
        {
            IsHit = true;
            HurtPlayer();
            Hide();
        }
    }

    private void HurtPlayer()
    {
        if (IsPercentage)
        {
            settings.Cursor.HurtFocusValuePercentage(Damage);
        }
        else
        {
            settings.Cursor.HurtFocusValue(Damage);
        }

        if (!settings.Shield.IsShielded)
        {
            PlayHurtPS();
        }

        StopIdlePS();
    }

    private void PlayHurtPS()
    {
        PsHurt?.SetParent(settings.FocusEvent);
        PsHurt?.Play(destroy: true);
    }

    private void StopIdlePS()
    {
        PsIdle?.SetParent(settings.FocusEvent);
        PsIdle?.Stop(destroy: true);
    }
}
