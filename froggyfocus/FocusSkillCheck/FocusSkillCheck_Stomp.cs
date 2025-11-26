using Godot;
using System.Collections;

public partial class FocusSkillCheck_Stomp : FocusSkillCheck
{
    [Export]
    public Vector2 SpeedRange;

    [Export]
    public Node3D TargetParent;

    [Export]
    public Node3D TargetPosition;

    [Export]
    public AnimationPlayer AnimationPlayer;

    public override void Clear()
    {
        base.Clear();
        Target.SetParent(FocusEvent);
    }

    protected override void Stop()
    {
        base.Stop();
        AnimationPlayer.Stop();
    }

    protected override IEnumerator Run()
    {
        TargetPosition.GlobalPosition = Target.GlobalPosition;
        Target.SetParent(TargetParent);

        AnimationPlayer.SpeedScale = SpeedRange.Range(Difficulty);
        yield return AnimationPlayer.PlayAndWaitForAnimation("stomp");

        Clear();
    }

    public void Hurt()
    {
        FocusEvent.Cursor.HurtFocusValuePercentage(0.2f);
    }
}
