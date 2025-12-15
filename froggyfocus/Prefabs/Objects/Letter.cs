using Godot;
using System;
using System.Collections;

public partial class Letter : Area3D
{
    [Export]
    public string Id;

    [Export]
    public AnimationPlayer AnimationPlayer;

    [Export]
    public AnimationPlayer AnimationPlayer_Open;

    public event Action OnMouseEnter, OnMouseExit;

    public override void _MouseEnter()
    {
        base._MouseEnter();
        OnMouseEnter?.Invoke();
    }

    public override void _MouseExit()
    {
        base._MouseExit();
        OnMouseExit?.Invoke();
    }

    public IEnumerator AnimateRaise()
    {
        return AnimationPlayer.PlayAndWaitForAnimation("raise");
    }

    public IEnumerator AnimateLower()
    {
        return AnimationPlayer.PlayAndWaitForAnimation("lower");
    }

    public IEnumerator AnimateOpen()
    {
        yield return AnimationPlayer_Open.PlayAndWaitForAnimation("open");
        yield return AnimationPlayer.PlayAndWaitForAnimation("hide");
    }
}
