using FlawLizArt.Animation.StateMachine;
using Godot;

public partial class WormCharacter : FocusCharacter
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
        var idle = Animation.CreateAnimation("Armature|idle", true);
        var walking = Animation.CreateAnimation("Armature|walking", true);

        Animation.Connect(idle, walking, param_moving.WhenTrue());
        Animation.Connect(walking, idle, param_moving.WhenFalse());

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
