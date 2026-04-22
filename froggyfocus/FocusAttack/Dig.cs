using System.Collections;

namespace FlawLizArt.FocusEvent;

public partial class Dig : FocusAttack
{
    protected override void CursorEnter()
    {
        base.CursorEnter();
        var duration = rng.RandfRange(2, 6);
        WaitFor(duration, Run);
    }

    private void Run()
    {
        if (!Target.HasCursor) return;

        this.StartCoroutine(Cr, "run");
        IEnumerator Cr()
        {
            StartState();
            HurtFocusValue(0.1f);
            DisruptCursorFocus();

            Target.Animate_Exclamation();
            yield return Target.Animate_DigDown();

            var position = Target.GetNextPosition();
            Target.GlobalPosition = position;

            yield return Target.Animate_DigUp();

            EndState();
        }
    }
}
