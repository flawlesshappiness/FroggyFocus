using Godot;
using System.Collections;

public partial class FocusSkillCheck_FreezeTeleport : FocusSkillCheck
{
    [Export]
    public AudioStreamPlayer SfxFreeze;

    [Export]
    public Node3D ErrorLabel;

    public override void Clear()
    {
        base.Clear();
    }

    protected override void Stop()
    {
        base.Stop();
        ErrorLabel.Hide();
    }

    protected override IEnumerator Run()
    {
        var id = nameof(FocusSkillCheck_FreezeTeleport);
        Scene.PauseLock.SetLock(id, true);

        SfxFreeze.Play();
        yield return new WaitForSecondsUnscaled(rng.RandfRange(0.2f, 0.5f));
        SfxFreeze.Stop();

        ErrorLabel.GlobalPosition = Target.GlobalPosition;
        ErrorLabel.Show();

        Target.GlobalPosition = Target.GetRandomPosition();
        Scene.PauseLock.SetLock(id, false);

        Clear();
    }
}
