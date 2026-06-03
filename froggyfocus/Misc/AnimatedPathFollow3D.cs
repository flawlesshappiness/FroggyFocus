using Godot;
using System.Collections;

[GlobalClass]
public partial class AnimatedPathFollow3D : PathFollow3D
{
    [Export]
    public float Duration = 1f;

    [Export]
    public float SmoothingSpeed = 10f;

    [Export]
    public EasingFunctions.Ease Ease;

    [Export]
    public Camera3D Camera;

    [Export]
    public Node3D Target;

    public Coroutine Animate()
    {
        ProgressRatio = 0.0f;
        Camera.GlobalTransform = Target.GlobalTransform;

        return this.StartCoroutine(Cr, "animate");
        IEnumerator Cr()
        {
            var curve = EasingFunctions.GetEasingFunction(Ease);
            yield return LerpEnumerator.Lerp01(Duration, f =>
            {
                var t = curve(0f, 1f, f);
                ProgressRatio = t;
                UpdateCameraTransform();
            });
        }
    }

    private void UpdateCameraTransform()
    {
        Camera.GlobalTransform = Camera.GlobalTransform.InterpolateWith(Target.GlobalTransform, SmoothingSpeed * GameTime.DeltaTime);
    }
}
