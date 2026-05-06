using System.Collections;

namespace FlawLizArt.FocusEvent;

public partial class Flower : TrapAttack<CursorFlower>
{
    protected override IEnumerator AnimateCharacterTrap()
    {
        Target.Animate_Exclamation();
        yield return Target.Animate_DiveDown();

        var position = Target.GetNextPosition();
        Target.GlobalPosition = position;

        yield return Target.Animate_DiveUp();
    }
}
