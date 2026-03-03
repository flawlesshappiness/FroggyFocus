using Godot;
using System.Collections;

public partial class AnimatedOverlay : ColorRect
{
    [Export]
    public AnimationPlayer AnimationPlayer;

    public Coroutine AnimateShow(float duration, Color color)
    {
        Color = color;
        return Animate("show", duration);
    }

    public Coroutine AnimateHide(float duration, Color color)
    {
        Color = color;
        return Animate("hide", duration);
    }

    public Coroutine AnimateBehindShow()
    {
        return AnimateShow(0.25f, Colors.Black.SetA(0.5f));
    }

    public Coroutine AnimateBehindHide()
    {
        return AnimateHide(0.25f, Colors.Black.SetA(0.5f));
    }

    public Coroutine AnimateFrontShow()
    {
        return AnimateShow(0.5f, Colors.Black);
    }

    public Coroutine AnimateFrontHide()
    {
        return AnimateHide(0.5f, Colors.Black);
    }

    public void ShowImmediate()
    {
        AnimationPlayer.Stop();
        Color = Colors.Black;
        Modulate = Colors.White;
    }

    private Coroutine Animate(string animation, float duration)
    {
        AnimationPlayer.SpeedScale = 1f / duration;
        return Animate(animation);
    }

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
