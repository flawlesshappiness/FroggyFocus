using FlawLizArt.Animation.StateMachine;

public partial class PixelFrogCharacter : FrogCharacter
{
    private BoolParameter param_moving = new("moving", false);
    private BoolParameter param_charging = new("charging", false);
    private BoolParameter param_jumping = new("jumping", false);
    private BoolParameter param_cover_eyes = new("cover_eyes", false);

    protected override void InitializeAnimations()
    {
        base.InitializeAnimations();

        if (DisableAnimationStates) return;

        var idle = Animation.CreateAnimation("idle", true);
        var walking = Animation.CreateAnimation("walking", true);
        var jump = Animation.CreateAnimation("jump", true);
        var charging = Animation.CreateAnimation("charging", true);
        var cover_eyes = Animation.CreateAnimation("cover_eyes", true);

        Animation.Connect(idle, walking, param_moving.WhenTrue());
        Animation.Connect(walking, idle, param_moving.WhenFalse());

        Animation.Connect(idle, charging, param_charging.WhenTrue());
        Animation.Connect(walking, charging, param_charging.WhenTrue());

        Animation.Connect(charging, jump, param_jumping.WhenTrue());
        Animation.Connect(jump, idle, param_jumping.WhenFalse());

        Animation.Connect(idle, cover_eyes, param_cover_eyes.WhenTrue());
        Animation.Connect(cover_eyes, idle, param_cover_eyes.WhenFalse());

        Animation.Start(idle.Node);
    }

    public override void SetMoving(bool moving)
    {
        base.SetMoving(moving);
        param_moving.Set(moving);
    }

    public override void SetJumping(bool jumping)
    {
        base.SetJumping(jumping);
        param_jumping.Set(jumping);
    }

    public override void SetCharging(bool charging)
    {
        base.SetCharging(charging);
        param_charging.Set(charging);
    }

    public override void SetCoveringEyes(bool cover_eyes)
    {
        base.SetCoveringEyes(cover_eyes);
        param_cover_eyes.Set(cover_eyes);
    }
}
