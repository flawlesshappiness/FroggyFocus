using FlawLizArt.Animation.StateMachine;
using Godot;
using System;
using System.Collections;

public partial class FrogCharacter : Character
{
    [Export]
    public string AnimationName_Prefix = "Armature|";

    [Export]
    public string AnimationName_Idle = "idle";

    [Export]
    public string AnimationName_Walk = "walking";

    [Export]
    public string AnimationName_Charge = "charging";

    [Export]
    public string AnimationName_Jump = "jump_start";

    [Export]
    public string AnimationName_JumpEnd = "jump_end";

    [Export]
    public string AnimationName_Fall = "falling";

    [Export]
    public string AnimationName_TongueOut = "tongue_out";

    [Export]
    public string AnimationName_TongueIn = "tongue_in";

    [Export]
    public string AnimationName_Searching = "searching";

    [Export]
    public string AnimationName_CoverEyes = "cover_eyes";

    [Export]
    public string AnimationName_UncoverEyes = "uncover_eyes";

    [Export]
    public bool DisableAnimationStates;

    [Export]
    public bool DisableAppearanceUpdates;

    [Export]
    public FrogTongue Tongue;

    [Export]
    public AnimationStateMachine Animation;

    [Export]
    public PlayerMoveSoundsGroup MoveSounds;

    [Export]
    public MeshInstance3D BodyMesh;

    [Export]
    public AppearanceAttachmentGroup HatAttachments;

    [Export]
    public AppearanceAttachmentGroup FaceAttachments;

    [Export]
    public AppearanceAttachmentGroup ParticlesAttachments;

    public bool IsHandOut { get; private set; }

    public event Action<string> OnAnimationStarted;

    private ShaderMaterial body_material;

    private AnimationState state_in_sand;
    private AnimationState state_falling;

    private BoolParameter param_moving = new BoolParameter("moving", false);
    private BoolParameter param_tongue_out = new BoolParameter("mouth_open", false);
    private BoolParameter param_jumping = new BoolParameter("jumping", false);
    private BoolParameter param_charging = new BoolParameter("charging", false);
    private BoolParameter param_searching = new BoolParameter("searching", false);
    private BoolParameter param_cover_eyes = new BoolParameter("cover_eyes", false);

    private bool is_charging;
    private bool is_moving;
    private bool is_jumping;

    public override void _Ready()
    {
        base._Ready();
        InitializeMesh();
        InitializeAnimations();

        if (!DisableAppearanceUpdates)
        {
            ClearAppearance();
            LoadAppearance();
        }

        CustomizeAppearanceControl.OnAppearanceChanged += AppearanceChanged;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        CustomizeAppearanceControl.OnAppearanceChanged -= AppearanceChanged;
    }

    private void InitializeAnimations()
    {
        Animation.Animator.AnimationStarted += AnimationStarted;

        if (DisableAnimationStates) return;

        var idle = Animation.CreateAnimation($"{AnimationName_Prefix}{AnimationName_Idle}", true);
        var walking = Animation.CreateAnimation($"{AnimationName_Prefix}{AnimationName_Walk}", true);
        walking.SpeedScale = 1.5f;
        var jump_start = Animation.CreateAnimation($"{AnimationName_Prefix}{AnimationName_Jump}", false);
        var jump_end = Animation.CreateAnimation($"{AnimationName_Prefix}{AnimationName_JumpEnd}", true);
        var jump_charge = Animation.CreateAnimation($"{AnimationName_Prefix}{AnimationName_Charge}", true);
        var tongue_out = Animation.CreateAnimation($"{AnimationName_Prefix}{AnimationName_TongueOut}", false);
        var tongue_in = Animation.CreateAnimation($"{AnimationName_Prefix}{AnimationName_TongueIn}", false);
        var searching = Animation.CreateAnimation($"{AnimationName_Prefix}{AnimationName_Searching}", false);
        var cover_eyes = Animation.CreateAnimation($"{AnimationName_Prefix}{AnimationName_CoverEyes}", false);
        var uncover_eyes = Animation.CreateAnimation($"{AnimationName_Prefix}{AnimationName_UncoverEyes}", false);

        state_falling = Animation.CreateAnimation($"{AnimationName_Prefix}{AnimationName_Fall}", false);
        state_in_sand = Animation.CreateAnimation("Armature|in_sand", true);

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

    private void InitializeMesh()
    {
        body_material = BodyMesh.GetActiveMaterial(0).Duplicate() as ShaderMaterial;
        BodyMesh.SetSurfaceOverrideMaterial(0, body_material);
    }

    public void ClearAppearance()
    {
        HatAttachments.Clear();
        FaceAttachments.Clear();
        ParticlesAttachments.Clear();
    }

    public void LoadAppearance()
    {
        if (Data.Game == null) return;
        LoadAppearance(Data.Game);
    }

    public void LoadAppearance(GameSaveData data)
    {
        LoadBodyColor(data);
        LoadHat(data);
        LoadFace(data);
        LoadParticles(data);
    }

    private void AppearanceChanged()
    {
        if (DisableAppearanceUpdates) return;
        LoadAppearance();
    }

    private void LoadBodyColor(GameSaveData game_data)
    {
        var albedo1 = game_data.FrogAppearanceData.BaseColor.Color;
        var albedo2 = game_data.FrogAppearanceData.CoatColor.Color;
        var albedo3 = game_data.FrogAppearanceData.PatternColor.Color;
        var albedo4 = game_data.FrogAppearanceData.EyeColor.Color;
        body_material.SetShaderParameter("albedo1", albedo1);
        body_material.SetShaderParameter("albedo2", albedo2);
        body_material.SetShaderParameter("albedo3", albedo3);
        body_material.SetShaderParameter("albedo4", albedo4);

        var texture2 = AppearanceController.Instance.GetInfo(game_data.FrogAppearanceData.GetOrCreateAttachmentData(ItemCategory.BodyTop).Type).Texture;
        var texture3 = AppearanceController.Instance.GetInfo(game_data.FrogAppearanceData.GetOrCreateAttachmentData(ItemCategory.BodyPattern).Type).Texture;
        var texture4 = AppearanceController.Instance.GetInfo(game_data.FrogAppearanceData.GetOrCreateAttachmentData(ItemCategory.BodyEye).Type).Texture;
        body_material.SetShaderParameter("texture2", texture2);
        body_material.SetShaderParameter("texture3", texture3);
        body_material.SetShaderParameter("texture4", texture4);
    }

    private void LoadHat(GameSaveData game_data)
    {
        var data = game_data.FrogAppearanceData.GetOrCreateAttachmentData(ItemCategory.Hat);
        HatAttachments.SetAttachment(data.Type, data.PrimaryColor, data.SecondaryColor);
    }

    private void LoadFace(GameSaveData game_data)
    {
        var data = game_data.FrogAppearanceData.GetOrCreateAttachmentData(ItemCategory.Face);
        FaceAttachments.SetAttachment(data.Type, data.PrimaryColor, data.SecondaryColor);
    }

    private void LoadParticles(GameSaveData game_data)
    {
        var data = game_data.FrogAppearanceData.GetOrCreateAttachmentData(ItemCategory.Particles);
        ParticlesAttachments.SetAttachment(data.Type, data.PrimaryColor, data.SecondaryColor);
    }

    public void SetMoving(bool moving)
    {
        if (is_moving == moving) return;

        is_moving = moving;
        param_moving.Set(moving);
    }

    public void SetJumping(bool jumping)
    {
        if (is_jumping == jumping) return;

        is_jumping = jumping;
        param_jumping.Set(jumping);
    }

    public void SetCharging(bool charging)
    {
        if (is_charging == charging) return;

        is_charging = charging;
        param_charging.Set(charging);
    }

    public void SetMouthOpen(bool open)
    {
        param_tongue_out.Set(open);
    }

    public void SetInSand()
    {
        Animation.SetCurrentState(state_in_sand.Node);
    }

    public void SetFalling()
    {
        Animation.SetCurrentState(state_falling.Node);
    }

    public void SetSearching(bool searching)
    {
        param_searching.Set(searching);
    }

    public void SetCoveringEyes(bool cover_eyes)
    {
        param_cover_eyes.Set(cover_eyes);
    }

    public Coroutine AnimateEatTarget(Node3D target)
    {
        return this.StartCoroutine(Cr, nameof(AnimateEatTarget));
        IEnumerator Cr()
        {
            param_tongue_out.Set(true);
            yield return Tongue.AnimateTongueTowards(target.GlobalPosition);
            Tongue.AttachToTongue(target);
            param_tongue_out.Set(false);
            yield return new WaitForSeconds(0.05f);
            yield return Tongue.AnimateTongueBack();
        }
    }

    private void AnimationStarted(StringName animName)
    {
        OnAnimationStarted?.Invoke($"{animName}");
    }
}
