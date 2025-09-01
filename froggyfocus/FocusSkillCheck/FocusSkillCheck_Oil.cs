using Godot;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class FocusSkillCheck_Oil : FocusSkillCheck
{
    [Export]
    public Vector2I CountRange;

    [Export]
    public Vector2I DurationRange;

    [Export]
    public PackedScene ClonePrefab;

    private SkillCheckOilClone target_clone;
    private List<SkillCheckOilClone> clones = new();

    public override void Clear()
    {
        base.Clear();
        target_clone = null;
        clones.ForEach(x => x.QueueFree());
        clones.Clear();
    }

    protected override void Stop()
    {
        base.Stop();
    }

    protected override IEnumerator Run()
    {
        // Hide target
        yield return AnimateHideTarget();

        // Create clones
        CreateClones();

        // Show clones
        clones.ForEach(x => x.AnimateShow());
        yield return new WaitForSeconds(0.25f);

        // Start moving clones
        clones.ForEach(x => x.StartMoving());

        // Wait for duration
        var duration = DurationRange.Range(Difficulty);
        yield return new WaitForSeconds(duration);

        // Stop moving clones
        clones.ForEach(x => x.StopMoving());

        // Hide clones
        clones.ForEach(x => x.AnimateHide());

        // Show target
        target_clone = null;
        yield return AnimateShowTarget();

        Clear();
    }

    private Coroutine AnimateHideTarget()
    {
        return this.StartCoroutine(Cr, "target_position");
        IEnumerator Cr()
        {
            var start = Target.GlobalPosition;
            var end = start.Set(y: -2);
            var curve = Curves.EaseInQuad;
            yield return LerpEnumerator.Lerp01(0.25f, f =>
            {
                var t = curve.Evaluate(f);
                Target.GlobalPosition = start.Lerp(end, t);
            });
            Target.Hide();
            Target.GlobalPosition = start;
        }
    }

    private Coroutine AnimateShowTarget()
    {
        return this.StartCoroutine(Cr, "target_position");
        IEnumerator Cr()
        {
            var start = Target.GlobalPosition.Set(y: -2);
            var end = start.Set(y: 0);
            var curve = Curves.EaseOutQuad;

            Target.GlobalPosition = start;
            yield return null;
            Target.Show();
            yield return LerpEnumerator.Lerp01(0.25f, f =>
            {
                var t = curve.Evaluate(f);
                Target.GlobalPosition = start.Lerp(end, t);
            });
        }
    }

    private List<SkillCheckOilClone> CreateClones()
    {
        var count = CountRange.Range(Difficulty);
        for (int i = 0; i < count; i++)
        {
            CreateClone();
        }

        target_clone = clones.First();
        target_clone.GlobalPosition = Target.GlobalPosition;
        target_clone.IsTarget = true;
        return clones;
    }

    private SkillCheckOilClone CreateClone()
    {
        var clone = ClonePrefab.Instantiate<SkillCheckOilClone>();
        clone.SetParent(this);
        clone.GlobalPosition = Target.GetRandomPosition();
        clone.Scale = Target.Scale;
        clone.Initialize(Target);
        clones.Add(clone);
        return clone;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        Process_TargetPosition();
    }

    private void Process_TargetPosition()
    {
        if (target_clone == null) return;
        Target.GlobalPosition = target_clone.GlobalPosition;
    }
}
