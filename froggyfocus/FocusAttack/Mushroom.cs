using Godot;
using System.Collections;

namespace FlawLizArt.FocusEvent;

public partial class Mushroom : FocusAttack
{
    [Export]
    public PackedScene MushroomPrefab;

    private Coroutine cr_run;

    protected override void Started()
    {
        base.Started();

        cr_run = this.StartCoroutine(Cr, "run");
        IEnumerator Cr()
        {
            while (true)
            {
                var duration_min = Mathf.Lerp(10f, 5f, Target.Difficulty);
                var duration_max = Mathf.Lerp(15f, 10f, Target.Difficulty);
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
        this.StartCoroutine(Cr, "spawn");
        IEnumerator Cr()
        {
            StartState();
            SpawnMushroom();
            Target.Animate_Exclamation();
            yield return new WaitForSeconds(0.5f);
            EndState();
        }
    }

    private void SpawnMushroom()
    {
        var mushroom = MushroomPrefab.Instantiate<FlashMushroom>();
        mushroom.SetParent(Target.FocusEvent);
        mushroom.Initialize(Target);
    }
}
