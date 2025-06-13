using Godot;
using System.Collections;

public partial class SkillCheckBomb : Node3D
{
    [Export]
    public AnimationPlayer AnimationPlayer;

    [Export]
    public AudioStreamPlayer3D SfxFuse;

    public void AnimateShow()
    {
        AnimationPlayer.Play("show");
        SfxFuse.Play();
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
        SfxFuse.FadeOut(0.25f);
        return AnimationPlayer.PlayAndWaitForAnimation("collect");
    }

    public IEnumerator AnimateExplode()
    {
        SfxFuse.FadeOut(0.25f);
        return AnimationPlayer.PlayAndWaitForAnimation("explode");
    }
}
