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

            if (Cursor.CurrentTarget == Target)
            {
                Cursor.HurtFocusValuePercentage(0.1f);
                Cursor.EndFocusTarget();
                AnimateMoveCursorAway();
            }

            yield return AnimateMoveTargetForward();
            EndState();
        }
    }

    private Coroutine AnimateMoveTargetForward()
    {
        return this.StartCoroutine(Cr, nameof(AnimateMoveTargetForward));
        IEnumerator Cr()
        {
            var duration = 0.25f;
            var start = Target.GlobalPosition;
            var dir = (Target.Character.GlobalBasis.Z * Vector3.Forward).Normalized() * 2f;
            var end = Target.GetApproximatePosition(start + dir);
            var curve = Curves.EaseOutQuad;
            yield return LerpEnumerator.Lerp01(duration, f =>
            {
                var t = curve.Evaluate(f);
                Target.GlobalPosition = start.Lerp(end, t);
            });
        }
    }

    private Coroutine AnimateMoveCursorAway()
    {
        return this.StartCoroutine(Cr, nameof(AnimateMoveCursorAway));
        IEnumerator Cr()
        {
            var duration = 0.4f;
            var start = Target.GlobalPosition;
            var end = start - (Target.Character.GlobalBasis.Z * Vector3.Forward).Normalized() * 2f;
            var curve = Curves.EaseOutQuad;

            FocusCursor.MoveLock.SetLock(nameof(WoodenStab), true);

            yield return LerpEnumerator.Lerp01(duration, f =>
            {
                var t = curve.Evaluate(f);
                Cursor.GlobalPosition = start.Lerp(end, t);
            });

            FocusCursor.MoveLock.SetLock(nameof(WoodenStab), false);
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
