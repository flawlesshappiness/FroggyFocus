using FlawLizArt.Animation.StateMachine;
using Godot;

public partial class FocusCharacter : Character
{
    [Export]
    public AnimationStateMachine Animation;

    [Export]
    public string IdleAnimation = "Armature|idle";

    [Export]
    public string WalkingAnimation = "Armature|walking";

    private BoolParameter param_moving = new BoolParameter("moving", false);

    public override void _Ready()
    {
        base._Ready();
        InitializeAnimations();
    }

    private void InitializeAnimations()
    {
        var idle = Animation.CreateAnimation("idle", IdleAnimation, true);
        var walking = Animation.CreateAnimation("walking", WalkingAnimation, true);

        Animation.Connect(idle, walking, param_moving.WhenTrue());
        Animation.Connect(walking, idle, param_moving.WhenFalse());

        Animation.Start(idle.Node);
    }

    public void SetMoving(bool moving)
    {
        if (moving)
        {
            MoveStarted();
        }
        else
        {
            MoveStopped();
        }
    }

    protected virtual void MoveStarted()
    {
        param_moving.Set(true);
    }

    protected virtual void MoveStopped()
    {
        param_moving.Set(false);
    }
}
