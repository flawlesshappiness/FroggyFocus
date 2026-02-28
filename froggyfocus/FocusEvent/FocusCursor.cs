using Godot;
using System;
using System.Linq;

public partial class FocusCursor : Node3D
{
    [Export]
    public Node3D RadiusNode;

    [Export]
    public AudioStreamPlayer SfxFocusStart;

    [Export]
    public AudioStreamPlayer SfxFocusEnd;

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
    public event Action OnMoveStarted;
    public event Action OnMoveEnded;

    public event Action<FocusTarget> OnTarget;
    public event Action<FocusTarget> OnTargetReleased;

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

    public void Stop()
    {
        EndFocusTarget();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        Process_Input();
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
        if (!FocusEvent.IsRunning) return;
        if (MoveLock.IsLocked) return;
        if (current_target != null) return;

        var target = GetNearTarget();
        if (target == null) return;

        current_target = target;
        current_target.SetHasCursor(true);
        Hide();

        SfxFocusStart.Play();

        OnTarget?.Invoke(current_target);
    }

    private void EndFocusTarget()
    {
        if (current_target == null) return;
        var target = current_target;
        current_target = null;

        Show();
        GlobalPosition = target.GlobalPosition;
        target.SetHasCursor(false);

        SfxFocusEnd.Play();

        OnTargetReleased?.Invoke(target);
    }

    private FocusTarget GetNearTarget()
    {
        return FocusEvent.Targets
            .Where(x => x.GlobalPosition.DistanceTo(GlobalPosition) < Radius && !x.IsFocusMax)
            .OrderBy(x => x.GlobalPosition.DistanceTo(GlobalPosition))
            .FirstOrDefault();
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
