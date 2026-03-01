using FlawLizArt.FocusEvent;
using System.Collections;

public partial class Dive : FocusAttack
{
    private Coroutine cr_start;

    protected override void CursorEnter()
    {
        base.CursorEnter();
        var duration = rng.RandfRange(2, 6);
        cr_start = WaitFor(duration, Run);
    }

    protected override void CursorExit()
    {
        base.CursorExit();
        Coroutine.Stop(cr_start);
    }

    private void Run()
    {
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
