using Godot;
using System.Collections;

namespace FlawLizArt.FocusEvent;

public partial class GlitchFlash : FocusAttack
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
                SpawnFlasher();
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

    private void SpawnFlasher()
    {
        if (FlashGlitch.CurrentFlasher != null) return;

        var node = FlasherPrefab.Instantiate<FlashGlitch>();
        node.SetParent(Target.FocusEvent);
        node.ClearPositionAndRotation();
        node.Initialize(Target);

        FlashGlitch.CurrentFlasher = node;
    }
}
