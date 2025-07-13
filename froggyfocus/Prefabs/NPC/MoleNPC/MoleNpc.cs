using FlawLizArt.Animation.StateMachine;
using Godot;

public partial class MoleNpc : Node3D
{
    [Export]
    public AnimationStateMachine Animation;

    [Export]
    public Node3D Mole;

    private BoolParameter param_show = new BoolParameter("show", false);

    public override void _Ready()
    {
        base._Ready();
        InitializeAnimations();
    }

    private void InitializeAnimations()
    {
        var idle = Animation.CreateAnimation("Armature|idle", true);
        var show = Animation.CreateAnimation("Armature|show", false);
        var hide = Animation.CreateAnimation("Armature|hide", false);
        var hidden = Animation.CreateAnimation("Armature|hidden", true);

        Animation.Connect(hidden, show, param_show.WhenTrue());
        Animation.Connect(show, idle);
        Animation.Connect(idle, hide, param_show.WhenFalse());
        Animation.Connect(hide, hidden);

        Animation.Start(hidden.Node);
    }

    public void AnimateShow()
    {
        param_show.Set(true);
    }

    public void AnimateHide()
    {
        param_show.Set(false);
    }

    public void TurnTowardsPosition(Vector3 position)
    {
        var direction = GlobalPosition.DirectionTo(position);
        TurnTowardsDirection(direction);
    }

    public void TurnTowardsDirection(Vector3 direction)
    {
        var y = Mathf.Atan2(direction.X, direction.Z);
        Mole.GlobalRotation = Mole.GlobalRotation.Set(y: y);
    }
}
