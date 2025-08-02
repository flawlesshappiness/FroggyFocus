using Godot;
using System.Collections;

public partial class SkillCheckBombProjectile : Node3D
{
    [Export]
    public ParticleEffectGroup PsProjectile;

    public IEnumerator WaitForMoveTowardsCursor(float duration, FocusCursor cursor)
    {
        PsProjectile.Play();

        yield return new WaitForSeconds(0.5f);

        var curve = Curves.EaseInQuad;
        var start = GlobalPosition;
        yield return LerpEnumerator.Lerp01(duration, f =>
        {
            var t = curve.Evaluate(f);
            var end = cursor.GlobalPosition;
            var position = start.Lerp(end, t);
            GlobalPosition = position;
        });

        PsProjectile.Stop();
    }
}
