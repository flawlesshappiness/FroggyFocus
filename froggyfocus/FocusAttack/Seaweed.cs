using Godot;
using System.Collections;

namespace FlawLizArt.FocusEvent;

public partial class Seaweed : FocusAttack
{
    [Export]
    public PackedScene PsFakeDigEffect;

    [Export]
    public PackedScene PsSpoutEffect;

    [Export]
    public AudioStreamPlayer SfxFakeDig;

    [Export]
    public AudioStreamPlayer SfxSpout;

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
                SetLock(true);
                yield return Target.Animate_DigDown();

                var position = Target.GetNextPosition();
                var dir = position - Target.GlobalPosition;
                var fake_position = Target.GlobalPosition - dir;

                var is_fake = rng.Randf() < 0.8f;
                if (is_fake)
                {
                    DoFakeDig(fake_position);
                    yield return new WaitForSeconds(1.0f);
                }

                Target.GlobalPosition = position;
                SetLock(false);
                yield return Target.Animate_DigUp();

                EndState();
            }
        }
    }

    private void SetLock(bool locked)
    {
        var id = nameof(Seaweed);
        Target.FocusLock.SetLock(id, locked);
    }

    private Coroutine DoFakeDig(Vector3 position)
    {
        return this.StartCoroutine(Cr, "fake_dig");
        IEnumerator Cr()
        {
            SfxFakeDig.Play();
            SpawnEffect(PsFakeDigEffect, position);
            yield return new WaitForSeconds(0.2f);
            SpawnEffect(PsSpoutEffect, position);
            SfxSpout.Play();
        }
    }

    private void SpawnEffect(PackedScene prefab, Vector3 position)
    {
        var effect = EffectGroup.Instantiate(prefab, Target.FocusEvent);
        effect.GlobalPosition = position;
    }
}