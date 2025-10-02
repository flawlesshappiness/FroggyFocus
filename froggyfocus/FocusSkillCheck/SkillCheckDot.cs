using Godot;
using System.Collections;

public partial class SkillCheckDot : Node3D
{
    [Export]
    public float Radius;

    [Export]
    public float DamagePercentage;

    [Export]
    public Vector2 Duration;

    [Export]
    public string ShowAnimation = "show";

    [Export]
    public string HideAnimation = "hide";

    [Export]
    public AnimationPlayer AnimationPlayer;

    public bool IsRunning { get; private set; }

    private FocusCursor Cursor => settings?.FocusEvent.Cursor;
    private bool NearCursor => GlobalPosition.DistanceTo(Cursor.GlobalPosition) < Cursor.Radius + Radius;

    private bool can_hurt;
    private float time_hurt;
    private Settings settings;

    public class Settings
    {
        public FocusEvent FocusEvent { get; set; }
    }

    public Coroutine StartDot(Settings settings)
    {
        IsRunning = true;
        this.settings = settings;
        var duration = Duration.Range(settings.FocusEvent.Target.Difficulty);
        return this.StartCoroutine(Cr, "dot");

        IEnumerator Cr()
        {
            yield return AnimateShow();
            can_hurt = true;
            yield return new WaitForSeconds(duration);
            can_hurt = false;
            yield return AnimateHide();

            IsRunning = false;
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (!can_hurt) return;
        if (GameTime.Time < time_hurt) return;

        if (NearCursor)
        {
            time_hurt = GameTime.Time + 0.5f;
            Cursor.HurtFocusValuePercentage(DamagePercentage);
        }
    }

    public Coroutine AnimateShow()
    {
        return Animate(ShowAnimation);
    }

    public Coroutine AnimateHide()
    {
        return Animate(HideAnimation);
    }

    private Coroutine Animate(string animation)
    {
        return this.StartCoroutine(Cr, "animate");
        IEnumerator Cr()
        {
            yield return AnimationPlayer.PlayAndWaitForAnimation(animation);
        }
    }
}
