using FlawLizArt.FocusEvent;
using Godot;
using System.Collections;

public partial class Crystal : FocusAttack
{
    [Export]
    public Vector2I HitCount = new Vector2I(3, 6);

    [Export]
    public PackedScene CrystalPrefab;

    protected override void CursorEnter()
    {
        base.CursorEnter();
        var duration = rng.RandfRange(2, 6);
        WaitFor(duration, Run);
    }

    private void Run()
    {
        if (!Target.HasCursor) return;

        this.StartCoroutine(Cr, "run");
        IEnumerator Cr()
        {
            StartState();
            HurtFocusValue(0.1f);
            DisruptCursorFocus();
            Spawn();

            Target.Animate_Exclamation();

            yield return AnimateMoveTargetForward();

            EndState();
        }
    }

    private void Spawn()
    {
        var hit_count = HitCount.Range(Target.Difficulty);
        var node = CrystalPrefab.Instantiate<CursorCrystal>();
        node.SetParent(Target.FocusEvent);
        node.Initialize(hit_count, Target.FocusEvent);
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
}
