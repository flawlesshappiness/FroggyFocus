using Godot;
using System;
using System.Linq;

namespace FlawLizArt.FocusEvent;

public partial class FocusCursor : Node3D
{
    [Export]
    public Node3D RadiusNode;

    [Export]
    public AudioStreamPlayer SfxFocusStart;

    [Export]
    public AudioStreamPlayer SfxFocusEnd;

    [Export]
    public AudioStreamPlayer SfxFocusHurt;

    public float Radius { get; private set; }
    private Vector3 DesiredVelocity { get; set; }
    private FocusEvent FocusEvent { get; set; }
    private float MoveSpeed { get; set; }
    public bool HasTarget => CurrentTarget != null;
    public FocusTarget CurrentTarget { get; private set; }

    public static readonly MultiLock MoveLock = new();
    public static readonly MultiLock SlowLock = new();

    private bool moving;

    public event Action OnFocusStarted;
    public event Action OnFocusStopped;
    public event Action OnMoveStarted;
    public event Action OnMoveEnded;

    public event Action<FocusTarget> OnTarget;
    public event Action<FocusTarget> OnTargetReleased;
    public event Action OnDisrupt;

    public void Initialize(FocusEvent focus_event)
    {
        FocusEvent = focus_event;
        Hide();
    }

    public void Load()
    {
        Radius = 0.5f;
        RadiusNode.Scale = Vector3.One * Radius;
        MoveSpeed = UpgradeController.Instance.GetCurrentValue(UpgradeType.CursorSpeed);
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
        if (CurrentTarget != null) return;

        var target = GetNearTarget();
        if (target == null) return;
        if (target.FocusLock.IsLocked) return;

        CurrentTarget = target;
        CurrentTarget.SetHasCursor(true);
        Hide();

        SfxFocusStart.Play();

        OnTarget?.Invoke(CurrentTarget);
    }

    public void DisruptFocusTarget()
    {
        if (CurrentTarget == null) return;

        EndFocusTarget();
        OnDisrupt?.Invoke();
    }

    public void EndFocusTarget()
    {
        if (CurrentTarget == null) return;
        var target = CurrentTarget;
        CurrentTarget = null;

        Show();
        GlobalPosition = target.GlobalPosition;
        target.SetHasCursor(false);

        SfxFocusEnd.Play();

        OnTargetReleased?.Invoke(target);
    }

    private FocusTarget GetNearTarget()
    {
        return FocusEvent.Targets
            .Where(x => x.DistanceToCursor < Radius && !x.IsFocusMax)
            .OrderBy(x => x.DistanceToCursor)
            .FirstOrDefault();
    }

    public bool IsNearTarget()
    {
        return GetNearTarget() != null;
    }

    public void HurtFocusValuePercentage(float percentage)
    {
        if (CurrentTarget != null)
        {
            SfxFocusHurt.Play();
            CurrentTarget.HurtFocusValue(percentage);
        }
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
