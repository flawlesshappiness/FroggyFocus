using Godot;
using System.Collections;

public partial class SkillCheckBomb : Node3D
{
    [Export]
    public AnimationPlayer AnimationPlayer;

    public void AnimateShow()
    {
        AnimationPlayer.Play("show");
    }

    public void AnimateIdle()
    {
        AnimationPlayer.Play("idle");
    }

    public void AnimateIdleFast()
    {
        AnimationPlayer.Play("idle_fast");
    }

    public IEnumerator AnimateCollect()
    {
        return AnimationPlayer.PlayAndWaitForAnimation("collect");
    }

    public IEnumerator AnimateExplode()
    {
        return AnimationPlayer.PlayAndWaitForAnimation("explode");
    }
}
