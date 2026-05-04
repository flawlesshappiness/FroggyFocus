using System.Collections;

namespace FlawLizArt.FocusEvent;

public partial class Dive : FocusAttack
{
    private Coroutine cr_run;

    protected override void Started()
    {
        base.Started();
        Run();
    }

    protected override void Stopped()
    {
        base.Stopped();
        Coroutine.Stop(cr_run);
    }

    protected override void Caught()
    {
        base.Caught();
        Coroutine.Stop(cr_run);
    }

    private void Run()
    {
        cr_run = this.StartCoroutine(Cr, "run");
        IEnumerator Cr()
        {
            while (true)
            {
                yield return new WaitForSeconds(rng.RandfRange(4f, 8f));

                StartState();

                if (IsFocusTarget)
                {
                    HurtFocusValue(0.1f);
                    DisruptCursorFocus();
                }

                Target.Animate_Exclamation();
                yield return Target.Animate_DiveDown();

                var position = Target.GetNextPosition();
                Target.GlobalPosition = position;

                yield return Target.Animate_DiveUp();

                EndState();
            }
        }
    }
}
