using System.Collections;

namespace FlawLizArt.FocusEvent;

public partial class Flower : TrapAttack<CursorFlower>
{
    protected override IEnumerator AnimateCharacterTrap()
    {
        Target.Animate_Exclamation();

        SetLock(true);
        yield return Target.Animate_DiveDown();

        var position = Target.GetNextPosition();
        Target.GlobalPosition = position;

        SetLock(false);
        yield return Target.Animate_DiveUp();
    }

    private void SetLock(bool locked)
    {
        var id = nameof(Dive);
        Target.FocusLock.SetLock(id, locked);
    }
}
