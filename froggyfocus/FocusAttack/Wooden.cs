using Godot;
using System.Collections;

namespace FlawLizArt.FocusEvent;

public partial class Wooden : FocusAttack
{
    [Export]
    public PackedScene WoodPrefab;

    private Coroutine cr_run;
    private Coroutine cr_wait;

    protected override void CursorEnter()
    {
        base.CursorEnter();
        var duration = rng.RandfRange(2, 6);
        cr_wait = WaitFor(duration, Spawn);
    }

    protected override void CursorExit()
    {
        base.CursorExit();
        Coroutine.Stop(cr_wait);
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
            Target.Animate_Exclamation();

            var wood = SpawnWood();
            wood.Warn();

            var duration = Mathf.Lerp(1.5f, 0.5f, Target.Difficulty); // grace period
            yield return new WaitForSeconds(duration);

            wood.AnimateStab();

            if (IsFocusTarget)
            {
                HurtFocusValue(0.1f);
                DisruptCursorFocus();
                AnimateMoveCursorAway();
            }

            yield return AnimateMoveTargetForward();
            EndState();
        }
    }

    private WoodenStab SpawnWood()
    {
        var node = WoodPrefab.Instantiate<WoodenStab>();
        node.SetParent(Target.FocusEvent);
        node.Initialize(Target);
        return node;
    }
}
