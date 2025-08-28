using Godot;
using System.Collections;

public partial class SkillCheckSeaweedPatch : Node3D
{
    [Export]
    public AnimationPlayer AnimationPlayer;

    private FocusCursor cursor;
    private float radius;
    private bool is_inside;
    private bool is_running;

    public Coroutine Run(FocusCursor cursor, float duration)
    {
        this.cursor = cursor;

        return this.StartCoroutine(Cr, "start");
        IEnumerator Cr()
        {
            yield return AnimateShow();
            is_running = true;
            yield return new WaitForSeconds(duration);
            Stop();
            yield return AnimateHide();
        }
    }

    public void Stop()
    {
        is_inside = false;
        is_running = false;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (!is_running) return;

        if (IsInsideRadius(cursor.GlobalPosition) != is_inside)
        {
            is_inside = !is_inside;
            FocusCursor.SlowLock.SetLock(nameof(FocusSkillCheck_Seaweed), is_inside);
        }

        if (is_inside && !AnimationPlayer.IsPlaying())
        {
            AnimateWiggle();
        }
    }

    public void SetSize(float size)
    {
        Scale = Vector3.One * size;
        radius = size * 0.5f;
    }

    public bool IsInsideRadius(Vector3 position)
    {
        return position.DistanceTo(GlobalPosition) < radius;
    }

    public Coroutine AnimateShow() => Animate("show");
    public Coroutine AnimateHide() => Animate("hide");
    public Coroutine AnimateWiggle() => Animate("wiggle");

    private Coroutine Animate(string animation)
    {
        return this.StartCoroutine(Cr, "animate");
        IEnumerator Cr()
        {
            yield return AnimationPlayer.PlayAndWaitForAnimation(animation);
        }
    }
}
