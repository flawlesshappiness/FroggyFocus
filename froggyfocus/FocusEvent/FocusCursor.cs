using Godot;
using System;
using System.Linq;

public partial class FocusCursor : Node3D
{
    [Export]
    public Node3D RadiusNode;

    public float Radius { get; private set; }
    private Vector3 DesiredVelocity { get; set; }
    private FocusEvent FocusEvent { get; set; }
    private float MoveSpeed { get; set; }

    public static readonly MultiLock MoveLock = new();
    public static readonly MultiLock SlowLock = new();

    private bool moving;
    private FocusTarget current_target;

    public event Action OnFocusStarted;
    public event Action OnFocusStopped;
    public event Action OnFocusTarget;
    public event Action OnFocusTargetEnter;
    public event Action OnFocusTargetExit;
    public event Action OnFocusFilled;
    public event Action OnFocusEmpty;
    public event Action OnMoveStarted;
    public event Action OnMoveEnded;

    public void Initialize(FocusEvent focus_event)
    {
        FocusEvent = focus_event;
        Hide();
    }

    public void Load()
    {
        //Radius = UpgradeController.Instance.GetCurrentValue(UpgradeType.CursorRadius);
        Radius = 0.5f;
        RadiusNode.Scale = Vector3.One * Radius;
        MoveSpeed = 0.05f; //UpgradeController.Instance.GetCurrentValue(UpgradeType.CursorSpeed);

        /*
        var base_focus_value = 80f;
        var focus_value_override = target.Info.FocusValueOverride;
        var focus_value = focus_value_override > 0 ? focus_value_override : base_focus_value;

        FocusTickTime = 0.15f;
        FocusMax = focus_value * Mathf.Lerp(1f, 1.5f, target.Difficulty);
        FocusValue = FocusMax * UpgradeController.Instance.GetCurrentValue(UpgradeType.CursorStartValue);
        FocusTickAmount = UpgradeController.Instance.GetCurrentValue(UpgradeType.CursorTickAmount);
        FocusTickDecay = UpgradeController.Instance.GetCurrentValue(UpgradeType.CursorTickDecay);
        */
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
        var input = PlayerInput.GetMoveInput();
        DesiredVelocity = new Vector3(input.X, 0, input.Y);

        if (PlayerInput.Interact.Pressed)
        {
            StartFocusTarget();
        }
        else if (PlayerInput.Interact.Released)
        {
            EndFocusTarget();
        }
    }

    private void PhysicsProcess_MoveCursor(float delta)
    {
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

    private void StartFocusTarget()
    {
        if (current_target != null) return;

        var target = GetNearTarget();
        if (target == null) return;

        current_target = target;
        current_target.SetHasCursor(true);
        Hide();
    }

    private void EndFocusTarget()
    {
        if (current_target == null) return;

        Show();
        GlobalPosition = current_target.GlobalPosition;
        current_target.SetHasCursor(false);
        current_target = null;
    }

    private void Process_Target()
    {
        /*
        if (GameTime.Time < next_tick) return;
        next_tick = GameTime.Time + FocusTickTime;

        if (IsNearTarget())
        {
            //SetFocusValue(FocusValue + FocusTickAmount);
            //PlayFocusGainEffect();
            //PlayFocusChangedSFX(FocusValue);
            //AnimationPlayer_Gain.Play("BounceIn");
            //OnFocusTarget?.Invoke();
        }
        else
        {
            //SetFocusValue(FocusValue - FocusTickDecay);
            //PlayFocusChangedSFX(0);
        }
        */
    }

    private FocusTarget GetNearTarget()
    {
        return FocusEvent.Targets.FirstOrDefault(x => x.GlobalPosition.DistanceTo(GlobalPosition) < Radius);
    }

    public bool IsNearTarget()
    {
        return GetNearTarget() != null;
    }

    public void HurtFocusValue(float value)
    {
    }

    public void HurtFocusValuePercentage(float percentage)
    {
        /*
        percentage = Mathf.Clamp(percentage, 0, 1);
        var value = FocusMax * percentage;
        HurtFocusValue(value);
        */
    }

    public void AdjustFocusValue(float value)
    {
        //SetFocusValue(FocusValue + value);
    }

    private void SetFocusValue(float value)
    {
        /*
        FocusValue = Mathf.Clamp(value, 0f, FocusMax);
        var t = FocusValue / FocusMax;
        //FillNode.Scale = Vector3.One * t;

        if (FocusValue >= FocusMax)
        {
            Filled = true;
            //SfxFocusComplete.Play();
            OnFocusFilled?.Invoke();
        }
        else if (FocusValue <= 0)
        {
            Empty = true;
            OnFocusEmpty?.Invoke();
        }
        */
    }

    public void PlayFocusChangedSFX(float value)
    {
        /*
        var pitch_min = 0.5f;
        var pitch_max = 1.5f;
        var t = value / FocusMax;
        //SfxFocusChanged.PitchScale = Mathf.Lerp(pitch_min, pitch_max, t);
        */
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
