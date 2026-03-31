using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FlawLizArt.FocusEvent;

public partial class FocusTarget : Node3D
{
    [Export]
    public NavigationAgent3D NavAgent;

    [Export]
    public Node3D CharacterParent;

    [Export]
    public FocusCircle FocusCircle;

    [Export]
    public GpuParticles3D PsWaterRipples;

    [Export]
    public AnimationPlayer AnimationPlayer_Glow;

    [Export]
    public AnimationPlayer AnimationPlayer_Character;

    [Export]
    public AnimationPlayer AnimationPlayer_Exclamation;

    [Export]
    public PackedScene SmokeDisappearEffect;

    public FocusEvent FocusEvent { get; private set; }
    public FocusCharacterInfo Info { get; private set; }
    public FocusCharacter Character { get; private set; }
    public InventoryCharacterData CharacterData { get; private set; }
    public float Difficulty { get; private set; }
    public float Radius => CharacterData.Size * 0.5f;
    public bool HasCursor { get; private set; }
    public float FocusValue { get; private set; }
    public float FocusMax { get; private set; }
    public float FocusTick { get; private set; }
    public bool IsFocusMax { get; private set; }
    public bool IsCaught { get; private set; }
    public float MoveSpeed { get; private set; }
    public Vector2 MoveDistance { get; private set; }
    public Vector2 MoveDelay { get; private set; }
    public int MovePoints { get; private set; }

    public event Action OnCaught;
    public event Action OnCursorEnter;
    public event Action OnCursorExit;
    public event Action OnStarted;
    public event Action OnStopped;

    private RandomNumberGenerator rng = new();
    private Coroutine state_cr;
    private State state;
    private float time_cursor_tick;

    private List<FocusAttack> attacks = new();

    private const float TICK_TIME = 0.2f;

    public enum State
    {
        Idle,
        Moving,
        Attack
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        Process_HasCursor();
    }

    public void Initialize(FocusEvent focus_event)
    {
        this.FocusEvent = focus_event;
    }

    public void SetData(InventoryCharacterData data)
    {
        CharacterData = data;
        Info = FocusCharacterController.Instance.GetInfoFromPath(data.InfoPath);
        SetCharacter(Info);
        UpdateFocusValue();
        UpdateSwimmer();
        UpdateDifficulty();
        InitializeAttacks();

        ResetCharacterAnimation();
        ResetGlow();
    }

    private void InitializeAttacks()
    {
        attacks = this.GetNodesInChildren<FocusAttack>();
        foreach (var attack in attacks)
        {
            attack.Initialize(this);
        }
    }

    private void SetCharacter(FocusCharacterInfo info)
    {
        RemoveCharacter();

        Info = info;
        Character = info.Scene.Instantiate<FocusCharacter>();
        Character.Initialize(info);
        Character.SetParent(CharacterParent);
        Character.ClearPositionAndRotation();

        Scale = Vector3.One * CharacterData.Size;
    }

    private void UpdateFocusValue()
    {
        FocusCircle.SetFill(0);
        FocusValue = 0f;
        FocusMax = 100f;
        FocusTick = UpgradeController.Instance.GetCurrentValue(UpgradeType.TickAmount);
    }

    private void UpdateDifficulty()
    {
        Difficulty = Mathf.Clamp((CharacterData.Stars - 1) / 4f, 0, 1);

        UpdateMoveSpeed();
        UpdateMoveDistance();
        UpdateMoveDelay();
        UpdateMovePoints();
    }

    private void UpdateMoveSpeed()
    {
        MoveSpeed = Info.SpeedType switch
        {
            FocusCharacterSpeedType.Stationary => 0f,
            FocusCharacterSpeedType.Slow => Mathf.Lerp(0.5f, 1.0f, Difficulty),
            FocusCharacterSpeedType.Normal => Mathf.Lerp(1.0f, 2.0f, Difficulty),
            FocusCharacterSpeedType.Fast => Mathf.Lerp(2.0f, 3.0f, Difficulty),
            FocusCharacterSpeedType.Glitch => Mathf.Lerp(0.3f, 0.5f, Difficulty),
        };
    }

    private void UpdateMoveDistance()
    {
        MoveDistance = Info.DistanceType switch
        {
            FocusCharacterDistanceType.Short => new Vector2(Mathf.Lerp(0.25f, 0.5f, Difficulty), Mathf.Lerp(1.0f, 1.5f, Difficulty)),
            FocusCharacterDistanceType.Normal => new Vector2(Mathf.Lerp(0.5f, 1.0f, Difficulty), Mathf.Lerp(1.5f, 2.0f, Difficulty)),
            FocusCharacterDistanceType.Long => new Vector2(Mathf.Lerp(1.0f, 2.0f, Difficulty), Mathf.Lerp(2.0f, 3.0f, Difficulty)),
        };
    }

    private void UpdateMoveDelay()
    {
        MoveDelay = Info.DelayType switch
        {
            FocusCharacterDelayType.None => new Vector2(0.0f, 0.0f),
            FocusCharacterDelayType.Short => new Vector2(Mathf.Lerp(0.1f, 0.2f, Difficulty), Mathf.Lerp(0.4f, 0.5f, Difficulty)),
            FocusCharacterDelayType.Normal => new Vector2(Mathf.Lerp(0.5f, 1.0f, Difficulty), Mathf.Lerp(1.5f, 2.0f, Difficulty)),
            FocusCharacterDelayType.Long => new Vector2(Mathf.Lerp(1.0f, 1.5f, Difficulty), Mathf.Lerp(2.0f, 3.0f, Difficulty)),
        };
    }

    private void UpdateMovePoints()
    {
        if (Info.SpeedType == FocusCharacterSpeedType.Stationary)
        {
            MovePoints = 0;
        }
        else
        {
            MovePoints = Info.DistanceType switch
            {
                FocusCharacterDistanceType.Short => 0,
                FocusCharacterDistanceType.Normal => new Vector2I(2, 3).Range(Difficulty),
                FocusCharacterDistanceType.Long => new Vector2I(4, 5).Range(Difficulty),
                _ => 0,
            };
        }
    }

    private void RemoveCharacter()
    {
        if (Character == null) return;

        Character.QueueFree();
        Character = null;
    }

    public void SetHasCursor(bool has_cursor)
    {
        time_cursor_tick = GameTime.Time + TICK_TIME;
        HasCursor = has_cursor;
        FocusCircle.SetEyeVisible(has_cursor);

        if (has_cursor)
        {
            OnCursorEnter?.Invoke();
        }
        else
        {
            OnCursorExit?.Invoke();
        }
    }

    private void Process_HasCursor()
    {
        if (GameTime.Time < time_cursor_tick) return;
        time_cursor_tick += TICK_TIME;

        if (FocusEvent.IsCoveringEyes) return;

        if (HasCursor)
        {
            AdjustFocusValue(FocusTick);
            FocusCircle.AnimateBounce(FocusValue >= FocusMax);
        }
        else if (!IsFocusMax)
        {
            AdjustFocusValue(-1.0f);
        }

        if (!IsFocusMax)
        {
            var t = Mathf.Clamp(FocusValue / FocusMax, 0f, 1f);
            IsFocusMax = t >= 1.0f;
            FocusCircle.SetFill(t);
        }
    }

    public void HurtFocusValue(float perc)
    {
        var amount = FocusMax * perc;
        IsFocusMax = false;
        AdjustFocusValue(-Mathf.Abs(amount));
    }

    private void AdjustFocusValue(float amount)
    {
        SetFocusValue(FocusValue + amount);
    }

    private void SetFocusValue(float value)
    {
        FocusValue = Mathf.Clamp(value, 0f, FocusMax);
    }

    public void Caught()
    {
        IsCaught = true;
        OnCaught?.Invoke();
    }

    public void StartState()
    {
        SetState(State.Moving);
        OnStarted?.Invoke();
    }

    public void StopState()
    {
        Coroutine.Stop(state_cr);
        OnStopped?.Invoke();
    }

    public void SetState(State state)
    {
        this.state = state;
        var e = GetStateCr(state);
        state_cr = this.StartCoroutine(e, "state");
    }

    private IEnumerator GetStateCr(State state) => state switch
    {
        State.Idle => StateIdle(),
        State.Moving => StateMoving(),
        State.Attack => StateAttack(),
    };

    private IEnumerator StateIdle()
    {
        var duration = MoveDelay.Range(rng.Randf());
        if (duration > 0)
        {
            StopMoving();
            yield return new WaitForSeconds(duration);
        }

        SetState(State.Moving);
    }

    private IEnumerator StateMoving()
    {
        if (Info.SpeedType != FocusCharacterSpeedType.Stationary)
        {
            yield return WaitForMoveToRandomPosition();
        }

        if (Info.DelayType == FocusCharacterDelayType.None)
        {
            SetState(State.Moving);
        }
        else
        {
            SetState(State.Idle);
        }
    }

    private IEnumerator StateAttack()
    {
        while (true)
        {
            yield return null; // Wait for attack to set new state
        }
    }

    private IEnumerator WaitForMoveToRandomPosition()
    {
        NavAgent.TargetPosition = GetNextPosition();

        var time_safety = GameTime.Time + 1f;
        while (NavAgent.IsNavigationFinished() && GameTime.Time < time_safety)
            yield return null;

        Character.SetMoving(true);

        var curve = CalculateMoveCurve();
        yield return WaitForMoveThroughCurve(curve);
    }

    private IEnumerator WaitForMoveThroughCurve(Curve3D curve)
    {
        var is_glitch = Info.SpeedType == FocusCharacterSpeedType.Glitch;
        var velocity = Vector3.Zero;
        var length = curve.GetBakedLength();
        var dist = 0f;
        while (dist < length)
        {
            var speed_mul = is_glitch ? 1.0f : GameTime.DeltaTime;
            var sample = curve.SampleBaked(dist);
            velocity = sample - GlobalPosition;
            Move(velocity);
            dist += MoveSpeed * speed_mul;

            if (is_glitch)
            {
                Character.RotateToDirectionImmediate(velocity);
                yield return new WaitForSeconds(0.3f);
            }
            else
            {
                Character.StartFacingDirection(velocity);
                yield return null;
            }
        }

        velocity = curve.SampleBaked(length) - GlobalPosition;
        Move(velocity);
        yield return null;
    }

    public void StopMoving()
    {
        Character.SetMoving(false);
    }

    private void Move(Vector3 velocity)
    {
        GlobalPosition += velocity;
    }

    public Vector3 GetApproximatePosition(Vector3 position)
    {
        return NavigationServer3D.MapGetClosestPoint(NavigationServer3D.GetMaps().First(), position).Set(y: position.Y);
    }

    public Vector3 GetRandomPosition()
    {
        var center = FocusEvent.GlobalPosition;
        var rx = FocusEvent.Size.X;
        var rz = FocusEvent.Size.Y;
        var x = rng.RandfRange(-rx, rx);
        var z = rng.RandfRange(-rz, rz);
        var position = center + new Vector3(x, 0, z);
        return position;
    }

    public Vector3 GetNextDirection(Vector3? from = null)
    {
        var position = GetRandomPosition();
        var dir = (from ?? GlobalPosition).DirectionTo(position).Normalized();
        return dir;
    }

    public Vector3 GetNextPosition(Vector3? from = null)
    {
        var dir = GetNextDirection(from);
        var distance = MoveDistance.Range(rng.Randf());
        var position = GetApproximatePosition((from ?? GlobalPosition) + dir * distance);
        return position;
    }

    private void UpdateSwimmer()
    {
        var is_swimmer = Info.Tags.Contains(FocusCharacterTag.Water);
        PsWaterRipples.Emitting = is_swimmer;
    }

    private Curve3D CalculateMoveCurve()
    {
        var y = GlobalPosition.Y;
        var curve = new Curve3D();
        var nav_points = NavAgent.GetCurrentNavigationPath();
        var points = CalculateMovePoints(nav_points);

        // Create curve
        for (int i = 0; i < points.Length; i++)
        {
            var point = points[i].Set(y: y);

            if (i == 0) // First point
            {
                curve.AddPoint(point);
            }
            else // In-between points
            {
                var prev = points[i - 1].Set(y: y);
                var middle = prev.Lerp(point, 0.5f);
                var p_in = prev - middle;
                var p_out = point - middle;

                curve.AddPoint(middle, p_in, p_out);

                if (i == points.Length - 1) // Last point
                {
                    curve.AddPoint(point);
                }
            }
        }

        return curve;
    }

    private Vector3[] CalculateMovePoints(Vector3[] nav_points)
    {
        var points = new Vector3[MovePoints + 2];
        var curve = new Curve3D();
        nav_points.ForEach(x => curve.AddPoint(x));
        var length = curve.GetBakedLength();
        var dist_per_point = length / (MovePoints + 1);
        var is_offset_right = true;
        var move_offset = GetMovePointsOffset();

        points[0] = curve.SampleBaked(0);
        var prev_point = points[0];
        for (int i = 0; i < MovePoints; i++)
        {
            var point = curve.SampleBaked((i + 1) * dist_per_point);
            var dir = point - prev_point;
            var cross = dir.Cross(Vector3.Up).Normalized();
            var mul = is_offset_right ? 1 : -1;
            points[i + 1] = prev_point + dir + cross * mul * move_offset;

            prev_point = point;
            is_offset_right = !is_offset_right;
        }
        points[points.Length - 1] = curve.SampleBaked(length);

        return points;
    }

    private float GetMovePointsOffset() => Info.DistanceType switch
    {
        FocusCharacterDistanceType.Short => 0.05f,
        FocusCharacterDistanceType.Normal => 0.1f,
        FocusCharacterDistanceType.Long => 0.25f,
        _ => 0.1f
    };

    public void ResetGlow()
    {
        AnimationPlayer_Glow.Play("RESET");
    }

    public void HideGlow()
    {
        AnimationPlayer_Glow.Play("hide");
    }

    public void ResetCharacterAnimation()
    {
        AnimationPlayer_Character.Play("RESET");
    }

    public Coroutine Animate_DigDown()
    {
        return AnimationPlayer_Character.PlayAndWaitForAnimation("dig_down");
    }

    public Coroutine Animate_DigUp()
    {
        return AnimationPlayer_Character.PlayAndWaitForAnimation("dig_up");
    }

    public Coroutine Animate_DiveDown()
    {
        return AnimationPlayer_Character.PlayAndWaitForAnimation("dive_down");
    }

    public Coroutine Animate_DiveUp()
    {
        return AnimationPlayer_Character.PlayAndWaitForAnimation("dive_up");
    }

    public Coroutine Animate_Disappear()
    {
        var ps = ParticleEffectGroup.Instantiate(SmokeDisappearEffect, GetParentNode3D());
        ps.GlobalPosition = GlobalPosition;
        ps.Play();

        return AnimationPlayer_Character.PlayAndWaitForAnimation("disappear");
    }

    public Coroutine Animate_Scared()
    {
        return AnimationPlayer_Character.PlayAndWaitForAnimation("scared");
    }

    public Coroutine Animate_Unscared()
    {
        return AnimationPlayer_Character.PlayAndWaitForAnimation("unscared");
    }

    public Coroutine Animate_Exclamation()
    {
        return AnimationPlayer_Exclamation.PlayAndWaitForAnimation("show");
    }
}
