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
                var cr = patch.Run(FocusEvent.Cursor, DurationRange.Range(Difficulty));
                crs.Add(cr);
                yield return new WaitForSeconds(0.1f);
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
        var rx = 1;
        var rz = 0.5f;
        var x = rng.RandfRange(-rx, rx);
        var z = rng.RandfRange(-rz, rz);
        var position = center + new Vector3(x, 0, z);
        return position;
    }

    private SkillCheckSeaweedPatch CreatePatch()
    {
        var node = SeaweedPatchPrefab.Instantiate<SkillCheckSeaweedPatch>();
        node.SetParent(this);
        node.SetSize(SizeRange.Range(Difficulty));
        created_patches.Add(node);
        return node;
    }
}
