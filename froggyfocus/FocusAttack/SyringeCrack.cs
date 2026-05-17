using Godot;
using System.Collections;

namespace FlawLizArt.FocusEvent;

public partial class SyringeCrack : FocusAttack
{
    [Export]
    public PackedScene TrapPrefab;

    [Export]
    public EffectGroupSpawner WarnEffect;

    [Export]
    public EffectGroupSpawner CrackEffect;

    [Export]
    public EffectGroupSpawner SplatEffect;

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
                yield return new WaitForSeconds(rng.RandfRange(3f, 5f));

                StartState();

                Target.Animate_Exclamation();
                WarnEffect.Spawn();
                yield return new WaitForSeconds(0.2f);
                WarnEffect.Spawn();
                yield return new WaitForSeconds(0.5f);

                CrackEffect.Spawn();
                SplatEffect.Spawn();

                if (IsFocusTarget)
                {
                    SetLock(true);
                    HurtFocusValue(0.25f);
                    DisruptCursorFocus();

                    var angle = rng.RandfRange(-60f, 60f);
                    var dir = Target.BackDirection().Rotated(Vector3.Up, Mathf.DegToRad(angle));
                    yield return AnimateMoveCursorAway(dir);
                    SpawnTrap();
                    SetLock(false);
                }

                EndState();
            }
        }
    }

    private void SpawnTrap()
    {
        var trap = TrapPrefab.Instantiate<TrapObject>();
        trap.SetParent(Target.FocusEvent);
        trap.Initialize(10, Target.FocusEvent);
    }

    private void SetLock(bool locked)
    {
        var id = GetType().ToString();
        Target.FocusLock.SetLock(id, locked);
    }
}
