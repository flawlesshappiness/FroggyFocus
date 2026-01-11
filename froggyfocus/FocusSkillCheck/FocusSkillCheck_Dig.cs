using System.Collections;

public partial class FocusSkillCheck_Dig : FocusSkillCheck
{
    public override void Clear()
    {
        base.Clear();
        Target.ResetCharacterAnimation();
    }

    protected override IEnumerator Run()
    {
        yield return Target.Animate_DigDown();

        var position = Target.GetNextPosition();
        Target.GlobalPosition = position;

        yield return Target.Animate_DigUp();

        Clear();
    }
}
