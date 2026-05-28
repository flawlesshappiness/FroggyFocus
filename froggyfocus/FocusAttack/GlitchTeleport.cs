using Godot;
using System.Collections;

namespace FlawLizArt.FocusEvent;

public partial class GlitchTeleport : FocusAttack
{
    [Export]
    public AudioStreamPlayer SfxFreeze;

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
        cr_run = this.StartCoroutine(Cr, "run")
            .SetRunWhilePaused(true);
        IEnumerator Cr()
        {
            while (true)
            {
                yield return new WaitForSeconds(rng.RandfRange(4f, 8f));

                StartState();

                var id = nameof(GlitchTeleport) + GetInstanceId();
                Scene.PauseLock.SetLock(id, true);

                if (IsFocusTarget)
                {
                    HurtFocusValue(0.1f);
                    DisruptCursorFocus();
                }

                SetLock(true);

                SfxFreeze.Play();
                yield return new WaitForSecondsUnscaled(0.1f);
                SfxFreeze.Stop();

                var position = Target.GetNextPosition();
                Target.GlobalPosition = position;

                Scene.PauseLock.SetLock(id, false);
                SetLock(false);

                EndState();
            }
        }
    }

    private void SetLock(bool locked)
    {
        var id = nameof(GlitchTeleport);
        Target.FocusLock.SetLock(id, locked);
    }
}
