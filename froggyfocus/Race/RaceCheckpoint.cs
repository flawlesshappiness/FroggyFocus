using Godot;
using System;
using System.Collections;

public partial class RaceCheckpoint : Area3D
{
    [Export]
    public AnimationPlayer Animation;

    [Export]
    public Sprite3D FinishLine;

    public event Action OnPlayerEntered;

    public override void _Ready()
    {
        base._Ready();
        BodyEntered += PlayerEntered;
    }

    private void PlayerEntered(GodotObject go)
    {
        OnPlayerEntered?.Invoke();
    }

    public void AnimateShow()
    {
        this.StartCoroutine(Cr, "animate");
        IEnumerator Cr()
        {
            yield return Animation.PlayAndWaitForAnimation("show");
            yield return Animation.PlayAndWaitForAnimation("idle");
        }
    }

    public void AnimateHide()
    {
        Animation.Play("hide");
    }

    public void AnimateHideImmediate()
    {
        Animation.Play("hide_immediate");
    }

    public void SetFinish(bool is_finish)
    {
        FinishLine.Visible = is_finish;
    }
}
