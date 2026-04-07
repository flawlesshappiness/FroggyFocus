using Godot;
using System.Collections;

public partial class AnimatedPathFollow3D : PathFollow3D
{
    [Export]
    public float Duration;

    [Export]
    public EasingFunctions.Ease Ease;

    [Export]
    public Camera3D Camera;

    public Coroutine Animate()
    {
        return this.StartCoroutine(Cr, "animate");
        IEnumerator Cr()
        {
            var curve = EasingFunctions.GetEasingFunction(Ease);
            yield return LerpEnumerator.Lerp01(Duration, f =>
            {
                var t = curve(0f, 1f, f);
                ProgressRatio = t;
            });
        }
    }
}
