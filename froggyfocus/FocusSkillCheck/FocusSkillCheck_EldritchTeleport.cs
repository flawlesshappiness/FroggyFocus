using Godot;
using System.Collections;

public partial class FocusSkillCheck_EldritchTeleport : FocusSkillCheck
{
    [Export]
    public SkillCheckEldritchTeleportEye Eye;

    public override void _Ready()
    {
        base._Ready();
        Eye.OnTeleport += EyeTeleport;
    }

    protected override void Stop()
    {
        base.Stop();
        Eye.Stop();
    }

    public override void Clear()
    {
        base.Clear();
    }

    protected override IEnumerator Run()
    {
        StartTeleport();
        yield return null;
    }

    private Coroutine StartTeleport()
    {
        return this.StartCoroutine(Cr, "cr");
        IEnumerator Cr()
        {
            var angle = rng.RandfRange(-90, 90);
            var position = GlobalPosition + (Vector3.Forward * 2.5f).Rotated(Vector3.Up, angle);
            Eye.GlobalPosition = position;
            yield return Eye.Animate();
            Clear();
        }
    }

    private void EyeTeleport()
    {
        FocusEvent.Cursor.GlobalPosition = Eye.GlobalPosition.Set(y: FocusEvent.Cursor.GlobalPosition.Y);
    }
}
