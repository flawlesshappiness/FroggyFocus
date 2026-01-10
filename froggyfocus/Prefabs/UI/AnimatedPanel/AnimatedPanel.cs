using Godot;
using System.Collections;

public partial class AnimatedPanel : Control
{
    [Export]
    public AnimationPlayer AnimationPlayer;

    public override void _Ready()
    {
        base._Ready();
        Hide();
    }

    public Coroutine AnimatePopShow() => Animate("pop_show");
    public Coroutine AnimatePopHide() => Animate("pop_hide");
    public Coroutine AnimateMoveUp() => Animate("move_up");
    public Coroutine AnimateMoveDown() => Animate("move_down");
    public Coroutine AnimateGrow() => Animate("grow");
    public Coroutine AnimateShrink() => Animate("shrink");
    public Coroutine AnimateBounce() => Animate("bounce");
    public Coroutine AnimateFadeShow() => Animate("fade_show");
    public Coroutine AnimateFadeHide() => Animate("fade_hide");

    private Coroutine Animate(string animation)
    {
        return this.StartCoroutine(Cr)
            .SetRunWhilePaused();
        IEnumerator Cr()
        {
            AnimationPlayer.Stop(true);
            yield return AnimationPlayer.PlayAndWaitForAnimation(animation);
        }
    }
}
