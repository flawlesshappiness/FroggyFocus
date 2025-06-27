using Godot;
using System.Collections;

public partial class AnimatedOverlay : ColorRect
{
    [Export]
    public AnimationPlayer AnimationPlayer;

    public Coroutine AnimateBehindShow() => Animate("behind_show");
    public Coroutine AnimateBehindHide() => Animate("behind_hide");
    public Coroutine AnimateFrontShow() => Animate("front_show");
    public Coroutine AnimateFrontHide() => Animate("front_hide");

    private Coroutine Animate(string animation)
    {
        return this.StartCoroutine(Cr)
            .SetRunWhilePaused();
        IEnumerator Cr()
        {
            yield return AnimationPlayer.PlayAndWaitForAnimation(animation);
        }
    }
}
