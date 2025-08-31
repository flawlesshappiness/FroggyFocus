using Godot;
using System.Collections;
using System.Collections.Generic;

public partial class FocusSkillCheck_Garbage : FocusSkillCheck
{
    [Export]
    public Vector2I CountRange;

    [Export]
    public Vector2 SizeRange;

    [Export]
    public Vector2 SpeedRange;

    [Export]
    public Vector2 DurationRange;

    [Export]
    public PackedScene FloatyPrefrab;

    private Coroutine cr_run;
    private List<SkillCheckGarbageFloaty> created_floaties = new();

    public override void Clear()
    {
        base.Clear();
        created_floaties.ForEach(x => x.QueueFree());
        created_floaties.Clear();

        Coroutine.Stop(cr_run);
    }

    protected override void Stop()
    {
        base.Stop();
        Coroutine.Stop(cr_run);
        created_floaties.ForEach(x => x.HideIfVisible());
    }

    protected override IEnumerator Run()
    {
        cr_run = RunCr();
        yield return null;
    }

    private Coroutine RunCr()
    {
        return this.StartCoroutine(Cr, nameof(RunCr));
        IEnumerator Cr()
        {
            var crs = new List<Coroutine>();
            var count = CountRange.Range(Difficulty);
            for (int i = 0; i < count; i++)
            {
                var spawn = CreateFloaty();
                spawn.GlobalPosition = GetSpawnPosition();
                spawn.RotationDegrees = Vector3.Up * rng.RandfRange(0f, 360f);
                var dir = GetSpawnDirection(spawn.GlobalPosition);
                var duration = DurationRange.Range(Difficulty);
                var cr = spawn.Run(dir, duration);
                crs.Add(cr);
                yield return new WaitForSeconds(rng.RandfRange(0.5f, 1.0f));
            }

            foreach (var cr in crs)
            {
                yield return cr;
            }

            Clear();
        }
    }

    private Vector3 GetSpawnPosition()
    {
        var center = FocusEvent.GlobalPosition;
        var circ = rng.RandCircDirection();
        var dir = new Vector3(circ.X, 0, circ.Y) * 3f;
        var position = center + dir;
        return position;
    }

    private Vector3 GetSpawnDirection(Vector3 start)
    {
        var dir = start.DirectionTo(FocusEvent.Cursor.GlobalPosition);
        return dir.Normalized();
    }

    private SkillCheckGarbageFloaty CreateFloaty()
    {
        var size = SizeRange.Range(Difficulty);
        var speed = SpeedRange.Range(Difficulty);
        var node = FloatyPrefrab.Instantiate<SkillCheckGarbageFloaty>();
        node.SetParent(this);
        node.Initialize(FocusEvent.Cursor, size, speed);
        created_floaties.Add(node);
        return node;
    }
}
