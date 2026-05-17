using Godot;
using System.Collections;

namespace FlawLizArt.FocusEvent;

public partial class EldritchEyeFlash : FocusAttack
{
    [Export]
    public PackedScene FlasherPrefab;

    private Coroutine cr_run;

    protected override void Started()
    {
        base.Started();

        cr_run = this.StartCoroutine(Cr, "run");
        IEnumerator Cr()
        {
            while (true)
            {
                var duration_min = Mathf.Lerp(5f, 5f, Target.Difficulty);
                var duration_max = Mathf.Lerp(8f, 8f, Target.Difficulty);
                var duration = rng.RandfRange(duration_min, duration_max);
                yield return new WaitForSeconds(duration);
                Spawn();
            }
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

    private void Spawn()
    {
        StartState();
        SpawnFlasher();
        Target.Animate_Exclamation();
        EndState();
    }

    private void SpawnFlasher()
    {
        var node = FlasherPrefab.Instantiate<FlashEldritchEye>();
        node.SetParent(Target.FocusEvent);
        node.GlobalPosition = GetRandomPosition();
        node.Initialize(Target);
    }

    private Vector3 GetRandomPosition()
    {
        var x_mul = rng.Randf() < 0.5f ? 1 : -1;
        var x = rng.RandfRange(2f, 4f) * x_mul;
        var z = rng.RandfRange(-4f, 4f);
        return Target.FocusEvent.GlobalPosition.Set(x: x, z: z);
    }
}
