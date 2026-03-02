using FlawLizArt.FocusEvent;
using System.Collections;

public partial class Dive : FocusAttack
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
            StopCursorFocus();

            Target.Animate_Exclamation();
            yield return Target.Animate_DiveDown();

            var position = Target.GetNextPosition();
            Target.GlobalPosition = position;

            yield return Target.Animate_DiveUp();

            EndState();
        }
    }
}
