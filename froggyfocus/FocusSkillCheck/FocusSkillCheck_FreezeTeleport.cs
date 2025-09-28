using Godot;
using System.Collections;

public partial class FocusSkillCheck_FreezeTeleport : FocusSkillCheck
{
    [Export]
    public AudioStreamPlayer SfxFreeze;

    public override void Clear()
    {
        base.Clear();
    }

    protected override IEnumerator Run()
    {
        var id = nameof(FocusSkillCheck_FreezeTeleport);
        Scene.PauseLock.SetLock(id, true);

        SfxFreeze.Play();
        yield return new WaitForSecondsUnscaled(0.5f);
        SfxFreeze.Stop();

        Target.GlobalPosition = Target.GetRandomPosition();
        Scene.PauseLock.SetLock(id, false);
        Clear();
    }
}
