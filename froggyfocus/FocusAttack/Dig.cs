using System.Collections;

namespace FlawLizArt.FocusEvent;

public partial class Dig : FocusAttack
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
            StopCursorFocus();
            HurtFocusValue(0.1f);

            yield return Target.Animate_DigDown();

            var position = Target.GetNextPosition();
            Target.GlobalPosition = position;

            yield return Target.Animate_DigUp();

            EndState();
        }
    }
}
