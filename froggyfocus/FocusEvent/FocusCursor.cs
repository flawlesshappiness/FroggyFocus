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
    public AnimationPlayer AnimationPlayer_Gain;

    [Export]
    public AnimationPlayer AnimationPlayer_Hurt;

    [Export]
    public AudioStreamPlayer SfxFocusChanged;

    [Export]
    public AudioStreamPlayer SfxFocusHurt;

    [Export]
    public AudioStreamPlayer SfxFocusBlock;

    public bool InputEnabled { get; set; }
    public bool TickEnabled { get; set; }
    public FocusTarget Target { get; set; }
    public float Radius { get; private set; }
    private Vector3 DesiredVelocity { get; set; }
    private float DistanceToTarget => Target == null ? 0f : GlobalPosition.DistanceTo(Target.GlobalPosition) - Target.Radius;
    private bool IsTargetInRange => DistanceToTarget < Radius;
    private float FocusValue { get; set; }
    private float FocusMax { get; set; }
    private float FocusTickTime { get; set; }
    private float FocusTickAmount { get; set; }
    private float FocusTickDecay { get; set; }
    private float MoveSpeed { get; set; }
    private bool Filled { get; set; }
    private bool Empty { get; set; }

    public static readonly MultiLock FocusGainLock = new();
    public static readonly MultiLock MoveLock = new();
    public static readonly MultiLock ShieldLock = new();

    private float next_tick;
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

        FocusTickTime = 0.15f;
        FocusMax = target.Info.FocusValue;
        FocusValue = FocusMax * UpgradeController.Instance.GetCurrentValue(UpgradeType.CursorStartValue);
        FocusTickAmount = UpgradeController.Instance.GetCurrentValue(UpgradeType.CursorTickAmount);
        FocusTickDecay = UpgradeController.Instance.GetCurrentValue(UpgradeType.CursorTickDecay);
        MoveSpeed = UpgradeController.Instance.GetCurrentValue(UpgradeType.CursorSpeed);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        var fdelta = Convert.ToSingle(delta);
        Process_Input();
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

        if (PlayerInput.Interact.Released && ShieldLock.IsFree)
        {
            Shield.StartShield();
        }
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

        GlobalPosition += DesiredVelocity * MoveSpeed;
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
            Shield.PlayBlockEffect();
        }
        else
        {
            AdjustFocusValue(-value);
            SfxFocusHurt.Play();
            AnimationPlayer_Hurt.Stop();
            AnimationPlayer_Hurt.Play("hurt");
        }
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
}
