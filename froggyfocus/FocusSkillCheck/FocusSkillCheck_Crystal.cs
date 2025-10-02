using Godot;
using System.Collections;
using System.Collections.Generic;

public partial class FocusSkillCheck_Crystal : FocusSkillCheck
{
    [Export]
    public Vector2I Count;

    [Export]
    public Vector2 DotDistance;

    [Export]
    public PackedScene DotPrefab;

    private Coroutine cr_run;
    private List<SkillCheckDot> dots = new();

    public override void Clear()
    {
        base.Clear();
        dots.ForEach(x => x.QueueFree());
        dots.Clear();
        Coroutine.Stop(cr_run);
    }

    protected override IEnumerator Run()
    {
        yield return base.Run();
        cr_run = RunCr();
    }

    private Coroutine RunCr()
    {
        return this.StartCoroutine(Cr, nameof(RunCr));
        IEnumerator Cr()
        {
            var count = Count.Range(Difficulty);

            for (int i = 0; i < count; i++)
            {
                var dot = CreateDot();
                dot.GlobalPosition = RandomPosition();
                dot.RotationDegrees = new Vector3(0f, rng.RandfRange(0f, 90f), 0f);
                dot.StartDot(new SkillCheckDot.Settings { FocusEvent = FocusEvent });

                yield return new WaitForSeconds(rng.RandfRange(0.1f, 0.2f));
            }

            foreach (var dot in dots)
            {
                while (dot.IsRunning)
                {
                    yield return null;
                }
            }

            Clear();
        }
    }

    private Vector3 RandomPosition()
    {
        var center = Target.GlobalPosition;
        var position = Target.GetRandomPosition();
        var dir = center.DirectionTo(position).Normalized();
        var distance = DotDistance.Range(rng.RandfRange(0f, 1f));
        return center + dir * distance;
    }

    private SkillCheckDot CreateDot()
    {
        var dot = DotPrefab.Instantiate<SkillCheckDot>();
        dot.SetParent(this);
        dots.Add(dot);
        return dot;
    }
}
