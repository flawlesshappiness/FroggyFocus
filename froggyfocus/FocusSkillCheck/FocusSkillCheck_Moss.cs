using Godot;
using System.Collections;
using System.Collections.Generic;

public partial class FocusSkillCheck_Moss : FocusSkillCheck
{
    [Export]
    public Vector2I CountRange;

    [Export]
    public Vector2 DelayRange;

    [Export]
    public Vector2 DistanceRange;

    [Export]
    public PackedScene BushPrefab;

    [Export]
    public AudioStreamPlayer3D SfxBush;

    private List<Node3D> created_objects = new();

    public override void Clear()
    {
        base.Clear();
        created_objects.ForEach(x => x.QueueFree());
        created_objects.Clear();
    }

    protected override IEnumerator Run()
    {
        var count = CountRange.Range(Difficulty);
        var mosses = CreateMosses(count);
        var target_moss = CreateMoss(Target.GlobalPosition);

        SfxBush.Play();

        mosses.ForEach(x => x.AnimateShow());
        yield return target_moss.AnimateShow();
        Target.Hide();

        var selected_moss = mosses.Random();
        Target.GlobalPosition = selected_moss.GlobalPosition;

        var delay = DelayRange.Range(Difficulty);
        var time_wait_end = GameTime.Time + delay;
        while (GameTime.Time < time_wait_end && !FocusEvent.Cursor.IsTargetInRange)
        {
            yield return null;
        }

        SfxBush.Play();

        yield return selected_moss.AnimateShake();
        yield return new WaitForSeconds(0.5f);

        SfxBush.Play();

        Target.Show();
        mosses.ForEach(x => x.AnimateHide());
        yield return target_moss.AnimateHide();

        Clear();
    }

    private Vector3 RandomMossPosition(Vector3 center, float distance)
    {
        var position = Target.GetRandomPosition();
        return center + center.DirectionTo(position).Normalized() * distance * rng.RandfRange(0.5f, 1.0f);
    }

    private List<SkillCheckMossBush> CreateMosses(int count)
    {
        var list = new List<SkillCheckMossBush>();
        var distance = DistanceRange.Range(Difficulty);
        for (int i = 0; i < count; i++)
        {
            var position = RandomMossPosition(Target.GlobalPosition, distance);
            var node = CreateMoss(position);
            list.Add(node);
        }

        return list;
    }

    private SkillCheckMossBush CreateMoss(Vector3 position)
    {
        var node = BushPrefab.Instantiate<SkillCheckMossBush>();
        node.SetParent(this);
        node.GlobalPosition = position;
        node.GlobalRotationDegrees = Vector3.Up * rng.RandfRange(0, 360f);
        node.Scale = Vector3.One * rng.RandfRange(0.3f, 0.6f);
        created_objects.Add(node);
        return node;
    }
}
