using Godot;
using System.Collections;

public partial class AnimatedPanel : Control
{
    [Export]
    public AnimationPlayer AnimationPlayer;

    public Coroutine AnimatePopShow() => Animate("pop_show");
    public Coroutine AnimatePopHide() => Animate("pop_hide");
    public Coroutine AnimateMoveUp() => Animate("move_up");
    public Coroutine AnimateMoveDown() => Animate("move_down");
    public Coroutine AnimateGrow() => Animate("grow");
    public Coroutine AnimateShrink() => Animate("move_shrink");

    private Coroutine Animate(string animation)
    {
        return this.StartCoroutine(Cr);
        IEnumerator Cr()
        {
            yield return AnimationPlayer.PlayAndWaitForAnimation(animation);
        }
    }
}
