using Godot;
using System.Collections;

namespace FlawLizArt.FocusEvent;

public partial class TrapAttack<T> : FocusAttack
    where T : TrapObject
{
    [Export]
    public Vector2I HitCount = new Vector2I(3, 6);

    [Export]
    public PackedScene TrapPrefab;

    private Coroutine cr_run;
    private float time_delay;
    private float time_cooldown;
    private readonly Vector2 DelayMin = new Vector2(3, 2);
    private readonly Vector2 DelayMax = new Vector2(5, 3);
    private readonly Vector2 Cooldown = new Vector2(10, 5);

    protected override void Started()
    {
        base.Started();
        Run();
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

    protected override void CursorEnter()
    {
        base.CursorEnter();

        if (GameTime.Time > time_delay)
        {
            var delay_min = DelayMin.Range(Target.Difficulty);
            var delay_max = DelayMax.Range(Target.Difficulty);
            var duration = rng.RandfRange(delay_min, delay_max);
            time_delay = GameTime.Time + duration;
        }
    }

    private void Run()
    {
        cr_run = this.StartCoroutine(Cr, "run");
        IEnumerator Cr()
        {
            while (true)
            {
                while (GameTime.Time < time_cooldown)
                    yield return null;

                while (GameTime.Time < time_delay)
                    yield return null;

                if (IsFocusTarget)
                {
                    StartState();
                    HurtFocusValue(0.1f);
                    DisruptCursorFocus();
                    Spawn();

                    yield return AnimateCharacterTrap();

                    time_cooldown = GameTime.Time + Cooldown.Range(Target.Difficulty);
                    EndState();
                }
                else
                {
                    yield return null;
                }
            }
        }
    }

    protected virtual IEnumerator AnimateCharacterTrap()
    {
        Target.Animate_Exclamation();
        yield return AnimateMoveTargetForward();
    }

    private void Spawn()
    {
        var hit_count = HitCount.Range(Target.Difficulty);
        var node = TrapPrefab.Instantiate<T>();
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
