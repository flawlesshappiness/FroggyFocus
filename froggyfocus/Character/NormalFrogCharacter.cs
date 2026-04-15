using FlawLizArt.Animation.StateMachine;
using Godot;

public partial class NormalFrogCharacter : FrogCharacter
{
    private AnimationState state_falling;
    private AnimationState state_in_sand;
    private ShaderMaterial body_material;

    private BoolParameter param_moving = new BoolParameter("moving", false);
    private BoolParameter param_tongue_out = new BoolParameter("mouth_open", false);
    private BoolParameter param_jumping = new BoolParameter("jumping", false);
    private BoolParameter param_charging = new BoolParameter("charging", false);
    private BoolParameter param_searching = new BoolParameter("searching", false);
    private BoolParameter param_cover_eyes = new BoolParameter("cover_eyes", false);

    protected override void InitializeBody()
    {
        base.InitializeBody();
        body_material = BodyMesh.GetActiveMaterial(0).Duplicate() as ShaderMaterial;
        BodyMesh.SetSurfaceOverrideMaterial(0, body_material);
    }

    protected override void LoadBody(GameSaveData game_data)
    {
        base.LoadBody(game_data);
        var albedo = game_data.FrogAppearanceData.BaseColor.Color;
        body_material.SetShaderParameter("albedo", albedo);
    }

    protected override void InitializeAnimations()
    {
        base.InitializeAnimations();

        if (DisableAnimationStates) return;

        var prefix = "Armature|";
        var idle = Animation.CreateAnimation($"{prefix}idle", true);
        var walking = Animation.CreateAnimation($"{prefix}walking", true);
        walking.SpeedScale = 1.5f;
        var jump_start = Animation.CreateAnimation($"{prefix}jump_start", false);
        var jump_end = Animation.CreateAnimation($"{prefix}jump_end", true);
        var jump_charge = Animation.CreateAnimation($"{prefix}charging", true);
        var tongue_out = Animation.CreateAnimation($"{prefix}mouth_open", false);
        var tongue_in = Animation.CreateAnimation($"{prefix}mouth_closed", false);
        var searching = Animation.CreateAnimation($"{prefix}searching", false);
        var cover_eyes = Animation.CreateAnimation($"{prefix}cover_eyes", false);
        var uncover_eyes = Animation.CreateAnimation($"{prefix}uncover_eyes", false);

        state_falling = Animation.CreateAnimation($"{prefix}falling", false);
        state_in_sand = Animation.CreateAnimation($"{prefix}in_sand", true);

        Animation.Connect(idle, walking, param_moving.WhenTrue());
        Animation.Connect(walking, idle, param_moving.WhenFalse());

        Animation.Connect(idle, jump_charge, param_charging.WhenTrue());
        Animation.Connect(walking, jump_charge, param_charging.WhenTrue());

        Animation.Connect(idle, searching, param_searching.WhenTrue());
        Animation.Connect(searching, idle, param_searching.WhenFalse());

        Animation.Connect(idle, cover_eyes, param_cover_eyes.WhenTrue());
        Animation.Connect(cover_eyes, uncover_eyes, param_cover_eyes.WhenFalse());
        Animation.Connect(uncover_eyes, idle);

        Animation.Connect(idle, tongue_out, param_tongue_out.WhenTrue());
        Animation.Connect(tongue_out, tongue_in, param_tongue_out.WhenFalse());
        Animation.Connect(tongue_out, tongue_in, param_tongue_out.WhenFalse());
        Animation.Connect(tongue_in, idle);

        Animation.Connect(jump_charge, jump_start, param_jumping.WhenTrue());
        Animation.Connect(idle, jump_start, param_jumping.WhenTrue());
        Animation.Connect(walking, jump_start, param_jumping.WhenTrue());
        Animation.Connect(jump_start, jump_end);
        Animation.Connect(jump_end, idle, param_jumping.WhenFalse());

        Animation.Start(idle.Node);
    }

    public override void SetMoving(bool moving)
    {
        base.SetMoving(moving);
        param_moving.Set(moving);
    }

    public override void SetCharging(bool charging)
    {
        base.SetCharging(charging);
        param_charging.Set(charging);
    }

    public override void SetJumping(bool jumping)
    {
        base.SetJumping(jumping);
        param_jumping.Set(jumping);
    }

    public override void SetFalling()
    {
        base.SetFalling();
        Animation.SetCurrentState(state_falling.Node);
    }

    public override void SetTongueOut(bool open)
    {
        base.SetTongueOut(open);
        param_tongue_out.Set(open);
    }

    public override void SetSearching(bool searching)
    {
        base.SetSearching(searching);
        param_searching.Set(searching);
    }

    public override void SetCoveringEyes(bool cover_eyes)
    {
        base.SetCoveringEyes(cover_eyes);
        param_cover_eyes.Set(cover_eyes);
    }

    public void SetInSand()
    {
        Animation.SetCurrentState(state_in_sand.Node);
    }
}
