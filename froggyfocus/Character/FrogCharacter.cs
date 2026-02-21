using FlawLizArt.Animation.StateMachine;
using Godot;
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
    public string AnimationName_Fall = "falling";

    [Export]
    public bool DisableAnimationStates;

    [Export]
    public bool DisableAppearanceUpdates;

    [Export]
    public Node3D Tongue;

    [Export]
    public Marker3D TongueTargetMarker;

    [Export]
    public Marker3D TongueStartMarker;

    [Export]
    public AnimationStateMachine Animation;

    [Export]
    public SoundInfo SfxSwallow;

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

    private float time_hand_out;
    private Node3D _attached_target;

    private ShaderMaterial body_material;

    private AnimationState state_in_sand;
    private AnimationState state_falling;

    private BoolParameter param_moving = new BoolParameter("moving", false);
    private BoolParameter param_mouth_open = new BoolParameter("mouth_open", false);
    private BoolParameter param_jumping = new BoolParameter("jumping", false);
    private BoolParameter param_charging = new BoolParameter("charging", false);

    private BoolParameter param_right_hand_forward = new BoolParameter("right_hand_forward", false);
    private BoolParameter param_left_hand_forward = new BoolParameter("left_hand_forward", false);
    private BoolParameter param_right_hand_right = new BoolParameter("right_hand_right", false);
    private BoolParameter param_left_hand_left = new BoolParameter("left_hand_left", false);

    public override void _Ready()
    {
        base._Ready();
        InitializeMesh();
        InitializeAnimations();
        InitializeTongue();

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

    private void InitializeTongue()
    {
        Tongue.Scale = new Vector3(1, 1, 0);
        Tongue.Hide();
    }

    private void InitializeAnimations()
    {
        if (DisableAnimationStates) return;

        var idle = Animation.CreateAnimation($"{AnimationName_Prefix}{AnimationName_Idle}", true);
        var walking = Animation.CreateAnimation($"{AnimationName_Prefix}{AnimationName_Walk}", true);
        walking.SpeedScale = 1.5f;
        var jump_start = Animation.CreateAnimation($"{AnimationName_Prefix}{AnimationName_Jump}", false);
        state_falling = Animation.CreateAnimation($"{AnimationName_Prefix}{AnimationName_Fall}", true);
        var jump_charge = Animation.CreateAnimation($"{AnimationName_Prefix}{AnimationName_Charge}", true);
        var mouth_open = Animation.CreateAnimation("Armature|mouth_open", false);
        var mouth_close = Animation.CreateAnimation("Armature|mouth_close", false);

        var left_hand_forward_out = Animation.CreateAnimation("Armature|left_hand_forward_out", false);
        var left_hand_forward_in = Animation.CreateAnimation("Armature|left_hand_forward_in", false);
        var right_hand_forward_out = Animation.CreateAnimation("Armature|right_hand_forward_out", false);
        var right_hand_forward_in = Animation.CreateAnimation("Armature|right_hand_forward_in", false);

        var right_hand_right_out = Animation.CreateAnimation("Armature|right_hand_right_out", false);
        var right_hand_right_in = Animation.CreateAnimation("Armature|right_hand_right_in", false);
        var left_hand_left_out = Animation.CreateAnimation("Armature|left_hand_left_out", false);
        var left_hand_left_in = Animation.CreateAnimation("Armature|left_hand_left_in", false);

        state_in_sand = Animation.CreateAnimation("Armature|in_sand", true);

        Animation.Connect(idle, walking, param_moving.WhenTrue());
        Animation.Connect(walking, idle, param_moving.WhenFalse());

        Animation.Connect(idle, jump_charge, param_charging.WhenTrue());
        Animation.Connect(walking, jump_charge, param_charging.WhenTrue());

        Animation.Connect(idle, mouth_open, param_mouth_open.WhenTrue());
        Animation.Connect(right_hand_forward_out, mouth_open, param_mouth_open.WhenTrue());
        Animation.Connect(left_hand_forward_out, mouth_open, param_mouth_open.WhenTrue());
        Animation.Connect(right_hand_right_out, mouth_open, param_mouth_open.WhenTrue());
        Animation.Connect(left_hand_left_out, mouth_open, param_mouth_open.WhenTrue());
        Animation.Connect(mouth_open, mouth_close, param_mouth_open.WhenFalse());
        Animation.Connect(mouth_open, mouth_close, param_mouth_open.WhenFalse());
        Animation.Connect(mouth_close, idle);

        Animation.Connect(jump_charge, jump_start, param_jumping.WhenTrue());
        Animation.Connect(idle, jump_start, param_jumping.WhenTrue());
        Animation.Connect(walking, jump_start, param_jumping.WhenTrue());
        Animation.Connect(jump_start, state_falling);
        Animation.Connect(state_falling, idle, param_jumping.WhenFalse());

        Animation.Connect(idle, right_hand_forward_out, param_right_hand_forward.WhenTrue());
        Animation.Connect(idle, left_hand_forward_out, param_left_hand_forward.WhenTrue());
        Animation.Connect(idle, right_hand_right_out, param_right_hand_right.WhenTrue());
        Animation.Connect(idle, left_hand_left_out, param_left_hand_left.WhenTrue());

        Animation.Connect(right_hand_forward_out, right_hand_forward_in, param_right_hand_forward.WhenFalse());
        Animation.Connect(left_hand_forward_out, left_hand_forward_in, param_left_hand_forward.WhenFalse());
        Animation.Connect(right_hand_right_out, right_hand_right_in, param_right_hand_right.WhenFalse());
        Animation.Connect(left_hand_left_out, left_hand_left_in, param_left_hand_left.WhenFalse());

        Animation.Connect(right_hand_forward_in, idle);
        Animation.Connect(left_hand_forward_in, idle);
        Animation.Connect(right_hand_right_in, idle);
        Animation.Connect(left_hand_left_in, idle);

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

    public override void _Process(double delta)
    {
        base._Process(delta);
        Process_AttachedTarget();
    }

    public Coroutine AnimateEatTarget(Node3D target)
    {
        SetHandsBack();

        return this.StartCoroutine(Cr, nameof(AnimateEatTarget));
        IEnumerator Cr()
        {
            yield return AnimateTongueTowards(target.GlobalPosition);
            AttachToTongue(target);
            yield return AnimateTongueBack();
            SfxSwallow.Play(GlobalPosition);
        }
    }

    public Coroutine AnimateInteract(Node3D target)
    {
        return this.StartCoroutine(Cr, nameof(AnimateEatTarget));
        IEnumerator Cr()
        {
            var empty_length = 2.5f;
            var empty_position = GlobalPosition + Basis * (Vector3.Forward * empty_length + Vector3.Up * 0.25f);
            var position = target == null ? empty_position : target.GlobalPosition;

            if (target != null)
            {
                StartFacingPosition(position);
                yield return new WaitForSeconds(0.25f);
            }

            yield return AnimateTongueTowards(position);
            AnimateTongueBack();
        }
    }

    public Coroutine AnimateTongueTowards(Vector3 position)
    {
        Tongue.GlobalPosition = TongueStartMarker.GlobalPosition;
        Tongue.LookAt(position);
        Tongue.Show();

        var dist = Tongue.GlobalPosition.DistanceTo(position);
        return this.StartCoroutine(Cr, nameof(AnimateTongueTowards));
        IEnumerator Cr()
        {
            param_mouth_open.Set(true);
            yield return new WaitForSeconds(0.2f);

            var start = Tongue.Scale.Z;
            var end = dist;
            yield return LerpEnumerator.Lerp01(0.08f, f =>
            {
                var z = Mathf.Lerp(start, end, f);
                Tongue.Scale = new Vector3(1, 1, z);
            });
        }
    }

    public Coroutine AnimateTongueBack()
    {
        return this.StartCoroutine(Cr, nameof(AnimateTongueBack));
        IEnumerator Cr()
        {
            var start = Tongue.Scale.Z;
            var end = 0;
            yield return LerpEnumerator.Lerp01(0.1f, f =>
            {
                var z = Mathf.Lerp(start, end, f);
                Tongue.Scale = new Vector3(1, 1, z);
            });

            ClearTongueAttachement();
            Tongue.Hide();

            param_mouth_open.Set(false);
            yield return new WaitForSeconds(0.25f);
        }
    }

    public void AttachToTongue(Node3D target)
    {
        if (target == null) return;

        _attached_target = target;
        target.SetParent(this);
    }

    public void ClearTongueAttachement()
    {
        if (_attached_target == null) return;

        _attached_target.Disable();
        _attached_target = null;
    }

    private void Process_AttachedTarget()
    {
        if (_attached_target == null) return;

        _attached_target.GlobalPosition = TongueTargetMarker.GlobalPosition;
    }

    public void SetMoving(bool moving)
    {
        param_moving.Set(moving);
    }

    public void SetJumping(bool jumping)
    {
        param_jumping.Set(jumping);
    }

    public void SetCharging(bool charging)
    {
        param_charging.Set(charging);
    }

    public void SetMouthOpen(bool open)
    {
        param_mouth_open.Set(open);
    }

    public void SetHandToPosition(Vector3 position)
    {
        var dir = position - GlobalPosition;
        var forward = GlobalBasis * Vector3.Forward;
        var angle = Mathf.RadToDeg(forward.SignedAngleTo(dir, Vector3.Up));
        var right = angle < 0;
        var abs = Mathf.Abs(angle);
        var far = abs > 50;

        if (GameTime.Time < time_hand_out) return;
        time_hand_out = GameTime.Time + 10f;

        if (IsHandOut)
        {
            SetHandsBack();
        }
        else if (right)
        {
            if (far)
            {
                SetRightHandRight();
            }
            else
            {
                SetRightHandForward();
            }
        }
        else
        {
            if (far)
            {
                SetLeftHandLeft();
            }
            else
            {
                SetLeftHandForward();
            }
        }
    }

    public void SetHandsBack()
    {
        param_right_hand_forward.Set(false);
        param_right_hand_right.Set(false);
        param_left_hand_forward.Set(false);
        param_left_hand_left.Set(false);

        IsHandOut = false;
    }

    public void SetRightHandForward()
    {
        SetHandsBack();
        param_right_hand_forward.Set(true);
        IsHandOut = true;
    }

    public void SetRightHandRight()
    {
        SetHandsBack();
        param_right_hand_right.Set(true);
        IsHandOut = true;
    }

    public void SetLeftHandForward()
    {
        SetHandsBack();
        param_left_hand_forward.Set(true);
        IsHandOut = true;
    }

    public void SetLeftHandLeft()
    {
        SetHandsBack();
        param_left_hand_left.Set(true);
        IsHandOut = true;
    }

    public void SetInSand()
    {
        Animation.SetCurrentState(state_in_sand.Node);
    }

    public void SetFalling()
    {
        Animation.SetCurrentState(state_falling.Node);
    }
}
