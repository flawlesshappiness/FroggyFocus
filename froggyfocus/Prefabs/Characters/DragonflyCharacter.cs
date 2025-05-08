using FlawLizArt.Animation.StateMachine;
using Godot;

public partial class DragonflyCharacter : FocusCharacter
{
    [Export]
    public AnimationStateMachine Animation;

    private BoolParameter param_moving = new BoolParameter("moving", false);

    public override void _Ready()
    {
        base._Ready();
        InitializeAnimations();
    }

    private void InitializeAnimations()
    {
        var idle = Animation.CreateAnimation("Armature|flying", true);
        Animation.Start(idle.Node);
    }

    protected override void MoveStarted()
    {
        base.MoveStarted();
        param_moving.Set(true);
    }

    protected override void MoveStopped()
    {
        base.MoveStopped();
        param_moving.Set(false);
    }
}
