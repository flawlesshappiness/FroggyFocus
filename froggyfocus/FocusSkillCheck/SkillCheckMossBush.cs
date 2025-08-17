using Godot;
using System.Collections;

public partial class SkillCheckMossBush : Node3D
{
    [Export]
    public AnimationPlayer AnimationPlayer;

    public Coroutine AnimateShow()
    {
        return Animate("show");
    }

    public Coroutine AnimateHide()
    {
        return Animate("hide");
    }

    public Coroutine AnimateShake()
    {
        return Animate("shake");
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
