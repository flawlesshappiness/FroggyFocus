using Godot;
using System.Collections;
using System.Collections.Generic;

public partial class FocusSkillCheck_Seaweed : FocusSkillCheck
{
    [Export]
    public Vector2I CountRange;

    [Export]
    public Vector2 SizeRange;

    [Export]
    public Vector2 DurationRange;

    [Export]
    public PackedScene SeaweedPatchPrefab;

    private Coroutine cr_run;
    private List<SkillCheckSeaweedPatch> created_patches = new();

    public override void Clear()
    {
        base.Clear();
        created_patches.ForEach(x => x.QueueFree());
        created_patches.Clear();

        Coroutine.Stop(cr_run);

        FocusCursor.SlowLock.RemoveLock(nameof(FocusSkillCheck_Seaweed));
    }

    protected override void Stop()
    {
        base.Stop();
        Coroutine.Stop(cr_run);
        created_patches.ForEach(x => x.Stop());
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
                var patch = CreatePatch();
                patch.GlobalPosition = GetPatchPosition();
                patch.RotationDegrees = Vector3.Up * rng.RandfRange(0f, 360f);
                var dir = GetPatchDirection(patch.GlobalPosition);
                var cr = patch.Run(dir, DurationRange.Range(Difficulty));
                crs.Add(cr);
                yield return new WaitForSeconds(0.3f);
            }

            foreach (var cr in crs)
            {
                yield return cr;
            }

            Clear();
        }
    }

    private Vector3 GetPatchPosition()
    {
        var center = FocusEvent.GlobalPosition;
        var circ = rng.RandCircDirection();
        var dir = new Vector3(circ.X, 0, circ.Y) * 6f;
        var position = center + dir;
        return position;
    }

    private Vector3 GetPatchDirection(Vector3 start)
    {
        var center = GlobalPosition + Vector3.Right * rng.RandfRange(-3, 3);
        var dir = start.DirectionTo(center);
        return dir.Normalized();
    }

    private SkillCheckSeaweedPatch CreatePatch()
    {
        var size = SizeRange.Range(Difficulty);
        var speed = 0.8f;
        var node = SeaweedPatchPrefab.Instantiate<SkillCheckSeaweedPatch>();
        node.SetParent(this);
        node.Initialize(FocusEvent.Cursor, size, speed);
        created_patches.Add(node);
        return node;
    }
}
