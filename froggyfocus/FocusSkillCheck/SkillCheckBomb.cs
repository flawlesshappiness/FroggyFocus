using Godot;
using System.Collections;

public partial class SkillCheckBomb : Node3D
{
    [Export]
    public AnimationPlayer AnimationPlayer;

    public Coroutine AnimateShow() => Animate("show");
    public Coroutine AnimateIdle() => Animate("idle");
    public Coroutine AnimateIdleFast() => Animate("idle_fast");
    public Coroutine AnimateCollect() => Animate("collect");
    public Coroutine AnimateExplode() => Animate("explode");

    private Coroutine Animate(string animation)
    {
        return this.StartCoroutine(Cr);
        IEnumerator Cr()
        {
            yield return AnimationPlayer.PlayAndWaitForAnimation(animation);
        }
    }
}
