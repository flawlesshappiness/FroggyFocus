using FlawLizArt.FocusEvent;
using Godot;
using System.Collections;

public partial class RobotFlash : FocusAttack
{
    [Export]
    public PackedScene FlashPrefab;

    private Coroutine cr_run;

    protected override void Started()
    {
        base.Started();

        if (Target.Lives == 1)
        {
            Run();
        }
    }

    protected override void Caught()
    {
        base.Caught();
        Coroutine.Stop(cr_run);
    }

    protected override void Stopped()
    {
        base.Stopped();
        Coroutine.Stop(cr_run);
    }

    private void Run()
    {
        cr_run = this.StartCoroutine(Cr, "run");
        IEnumerator Cr()
        {
            while (true)
            {
                yield return new WaitForSeconds(3f);
                Spawn();
            }
        }
    }

    private void Spawn()
    {
        var flash = FlashPrefab.Instantiate<RobotEyes>();
        flash.SetParent(Target.FocusEvent);
        flash.Initialize(Target);
    }
}
