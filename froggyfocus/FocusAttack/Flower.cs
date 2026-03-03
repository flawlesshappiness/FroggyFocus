using Godot;
using System.Collections;

namespace FlawLizArt.FocusEvent;

public partial class Flower : FocusAttack
{
    [Export]
    public Vector2I HitCount = new Vector2I(3, 6);

    [Export]
    public PackedScene FlowerPrefab;

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
            StopCursorFocus();
            SpawnFlower();

            Target.Animate_Exclamation();
            yield return Target.Animate_DiveDown();

            var position = Target.GetNextPosition();
            Target.GlobalPosition = position;

            yield return Target.Animate_DiveUp();

            EndState();
        }
    }

    private void SpawnFlower()
    {
        var hit_count = HitCount.Range(Target.Difficulty);
        var flower = FlowerPrefab.Instantiate<CursorFlower>();
        flower.SetParent(Target.FocusEvent);
        flower.Initialize(hit_count, Target.FocusEvent);
    }
}
