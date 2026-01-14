using Godot;
using System;

public partial class FocusCursor : Node3D
{
    [Export]
    public FocusCursorShield Shield;

    [Export]
    public Node3D RadiusNode;

    [Export]
    public Node3D FillNode;

    [Export]
    public Node3D ParticleParent;

    [Export]
    public AnimationPlayer AnimationPlayer_Gain;

    [Export]
    public AnimationPlayer AnimationPlayer_Hurt;

    [Export]
    public AudioStreamPlayer SfxFocusChanged;

    [Export]
    public AudioStreamPlayer SfxFocusHurt;

    [Export]
    public AudioStreamPlayer SfxFocusBlock;

    [Export]
    public AudioStreamPlayer SfxFocusComplete;

    [Export]
    public PackedScene FocusGainEffect;

    [Export]
    public PackedScene FocusCompleteEffect;

    public bool InputEnabled { get; set; }
    public bool TickEnabled { get; set; }
    public FocusTarget Target { get; set; }
    public float Radius { get; private set; }
    public float ShieldPercentage => ShieldMax == 0 ? 1.0f : ShieldValue / ShieldMax;
    private Vector3 DesiredVelocity { get; set; }
    private float DistanceToTarget => Target == null ? 0f : GlobalPosition.DistanceTo(Target.GlobalPosition) - Target.Radius;
    public bool IsTargetInRange => DistanceToTarget < Radius;
    private float FocusValue { get; set; }
    private float FocusMax { get; set; }
    private float FocusTickTime { get; set; }
    private float FocusTickAmount { get; set; }
    private float FocusTickDecay { get; set; }
    private float MoveSpeed { get; set; }
    private float ShieldMax { get; set; }
    private float ShieldValue { get; set; }
    private float ShieldGain { get; set; }
    private bool Filled { get; set; }
    private bool Empty { get; set; }

    public static readonly MultiLock FocusGainLock = new();
    public static readonly MultiLock MoveLock = new();
    public static readonly MultiLock ShieldLock = new();
    public static readonly MultiLock SlowLock = new();

    private float next_tick;
    private float time_shield_cooldown;
    private bool moving;

    public event Action OnFocusStarted;
    public event Action OnFocusStopped;
    public event Action OnFocusTarget;
    public event Action OnFocusTargetEnter;
    public event Action OnFocusTargetExit;
    public event Action OnFocusFilled;
    public event Action OnFocusEmpty;
    public event Action OnMoveStarted;
    public event Action OnMoveEnded;

    public void Initialize(FocusTarget target)
    {
        Target = target;
        Hide();
    }

    public void Start(FocusTarget target)
    {
        Load(target);
        Filled = false;
        Empty = false;
        InputEnabled = true;
        TickEnabled = true;
        FocusGainLock.Clear();
        Show();
    }

    public void Stop()
    {
        Hide();
        InputEnabled = false;
    }

    public void Load(FocusTarget target)
    {
        Radius = UpgradeController.Instance.GetCurrentValue(UpgradeType.CursorRadius);
        RadiusNode.Scale = Vector3.One * Radius;

        var base_focus_value = 80f;
        var focus_value_override = target.Info.FocusValueOverride;
        var focus_value = focus_value_override > 0 ? focus_value_override : base_focus_value;

        FocusTickTime = 0.15f;
        FocusMax = focus_value * Mathf.Lerp(1f, 1.5f, target.Difficulty);
        FocusValue = FocusMax * UpgradeController.Instance.GetCurrentValue(UpgradeType.CursorStartValue);
        FocusTickAmount = UpgradeController.Instance.GetCurrentValue(UpgradeType.CursorTickAmount);
        FocusTickDecay = UpgradeController.Instance.GetCurrentValue(UpgradeType.CursorTickDecay);
        MoveSpeed = UpgradeController.Instance.GetCurrentValue(UpgradeType.CursorSpeed);

        ShieldMax = UpgradeController.Instance.GetCurrentValue(UpgradeType.ShieldMax);
        ShieldValue = ShieldMax;
        ShieldGain = 0.1f;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        var fdelta = Convert.ToSingle(delta);
        Process_Input();
        Process_Shield();
        Process_Target();
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        var fdelta = Convert.ToSingle(delta);
        PhysicsProcess_MoveCursor(fdelta);
    }

    private void Process_Input()
    {
        if (!InputEnabled) return;

        var input = PlayerInput.GetMoveInput();
        DesiredVelocity = new Vector3(input.X, 0, input.Y);
    }

    private void Process_Shield()
    {
        if (Shield.IsShielded)
        {
            SetShieldValue(ShieldValue - GameTime.DeltaTime);
        }
        else
        {
            SetShieldValue(ShieldValue + ShieldGain * GameTime.DeltaTime);
        }

        if (!InputEnabled) return;
        if (GameTime.Time < time_shield_cooldown) return;

        if (!Shield.IsShielded && PlayerInput.Interact.Held && ShieldLock.IsFree && ShieldValue > 0)
        {
            time_shield_cooldown = GameTime.Time + 0.25f;
            Shield.SetShieldOn(true);
        }
        else if (Shield.IsShielded && (!PlayerInput.Interact.Held || ShieldLock.IsLocked || ShieldValue <= 0))
        {
            time_shield_cooldown = GameTime.Time + 0.25f;
            Shield.SetShieldOn(false);
        }
    }

    private void SetShieldValue(float value)
    {
        ShieldValue = Mathf.Clamp(value, 0, ShieldMax);
    }

    private void PhysicsProcess_MoveCursor(float delta)
    {
        if (!InputEnabled) return;

        var wants_to_move = DesiredVelocity.Length() > 0;
        if (!moving && wants_to_move)
        {
            moving = true;
            MoveStarted();
        }
        else if (moving && !wants_to_move)
        {
            moving = false;
            MoveEnded();
        }

        if (MoveLock.IsLocked) return;

        var vel_mult = SlowLock.IsFree ? 1.0f : 0.5f;
        GlobalPosition += DesiredVelocity * MoveSpeed * vel_mult;
    }

    private void Process_Target()
    {
        if (!InputEnabled) return;
        if (!TickEnabled) return;
        if (Filled) return;
        if (Empty) return;

        if (GameTime.Time < next_tick) return;
        next_tick = GameTime.Time + FocusTickTime;

        if (IsTargetInRange)
        {
            if (FocusGainLock.IsLocked) return;

            SetFocusValue(FocusValue + FocusTickAmount);
            //PlayFocusGainEffect();
            PlayFocusChangedSFX(FocusValue);
            AnimationPlayer_Gain.Play("BounceIn");
            OnFocusTarget?.Invoke();
        }
        else
        {
            SetFocusValue(FocusValue - FocusTickDecay);
            PlayFocusChangedSFX(0);
        }
    }

    public void HurtFocusValue(float value)
    {
        if (Shield.IsShielded)
        {
            SfxFocusBlock.Play();
            Shield.PlayBlockAnimation();
        }
        else
        {
            AdjustFocusValue(-value);
            SfxFocusHurt.Play();
            AnimationPlayer_Hurt.Stop();
            AnimationPlayer_Hurt.Play("hurt");
        }
    }

    public void HurtFocusValuePercentage(float percentage)
    {
        percentage = Mathf.Clamp(percentage, 0, 1);
        var value = FocusMax * percentage;
        HurtFocusValue(value);
    }

    public void AdjustFocusValue(float value)
    {
        SetFocusValue(FocusValue + value);
    }

    private void SetFocusValue(float value)
    {
        if (Filled) return;
        if (Empty) return;

        FocusValue = Mathf.Clamp(value, 0f, FocusMax);
        var t = FocusValue / FocusMax;
        FillNode.Scale = Vector3.One * t;

        if (FocusValue >= FocusMax)
        {
            Filled = true;
            SfxFocusComplete.Play();
            OnFocusFilled?.Invoke();
        }
        else if (FocusValue <= 0)
        {
            Empty = true;
            OnFocusEmpty?.Invoke();
        }
    }

    public void PlayFocusChangedSFX(float value)
    {
        var pitch_min = 0.5f;
        var pitch_max = 1.5f;
        var t = value / FocusMax;
        SfxFocusChanged.PitchScale = Mathf.Lerp(pitch_min, pitch_max, t);
        SfxFocusChanged.Play();
    }

    private void MoveStarted()
    {
        OnMoveStarted?.Invoke();
    }

    private void MoveEnded()
    {
        OnMoveEnded?.Invoke();
    }

    private void PlayFocusGainEffect()
    {
        ParticleEffectGroup.Instantiate(FocusGainEffect, ParticleParent);
    }

    private void PlayFocusCompleteEffect()
    {
        var ps = ParticleEffectGroup.Instantiate(FocusCompleteEffect, GetParentNode3D());
        ps.GlobalPosition = ParticleParent.GlobalPosition;
    }
}
