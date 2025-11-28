using Godot;
using System.Collections;

public partial class FocusSkillCheck_Dig : FocusSkillCheck
{
    [Export]
    public Node3D TargetNode;

    [Export]
    public Node3D TargetParent;

    [Export]
    public AnimationPlayer AnimationPlayer;

    public override void Clear()
    {
        base.Clear();
        Target.SetParent(FocusEvent);
        Target.SetGlowVisible(true);
    }

    protected override IEnumerator Run()
    {
        Target.SetGlowVisible(false);
        TargetNode.GlobalPosition = Target.GlobalPosition;
        TargetNode.GlobalRotation = Target.Character.GlobalRotation;

        Target.SetParent(TargetParent);

        yield return AnimationPlayer.PlayAndWaitForAnimation("dig_down");

        var position = Target.GetRandomPosition();
        TargetNode.GlobalPosition = position;

        yield return AnimationPlayer.PlayAndWaitForAnimation("dig_up");

        Clear();
    }
}
