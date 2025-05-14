using Godot;
using System;

public partial class FocusCursor : Node3D
{
    [Export]
    public Node3D RadiusNode;

    [Export]
    public AnimationPlayer Animation;

    public bool InputEnabled { get; set; }
    public FocusTarget Target { get; set; }
    private Vector3 DesiredVelocity { get; set; }
    private float Radius { get; set; }
    private float DistanceToTarget => Target == null ? 0f : GlobalPosition.DistanceTo(Target.GlobalPosition) - Target.Radius;
    private bool IsTargetInRange => DistanceToTarget < Radius;
    private float FocusValue { get; set; }
    private float FocusTickTime { get; set; }
    private float FocusTickAmount { get; set; }
    private float FocusTickDecay { get; set; }
    private float MoveSpeed { get; set; }
    private float MoveFocusSpeed { get; set; }
    private bool Filled { get; set; }
    private bool Empty { get; set; }

    private float next_tick;

    public event Action OnFocusStarted;
    public event Action OnFocusStopped;
    public event Action OnFocusTarget;
    public event Action OnFocusTargetEnter;
    public event Action OnFocusTargetExit;
    public event Action OnFocusFilled;
    public event Action OnFocusEmpty;

    public void Initialize(FocusTarget target)
    {
        Target = target;
        Hide();
    }

    public void Start(FocusTarget target)
    {
        Load();
        Filled = false;
        Empty = false;
        InputEnabled = true;
        Show();
    }

    public void Stop()
    {
        Hide();
        InputEnabled = false;
    }

    public void Load()
    {
        Radius = StatsController.Instance.GetCurrentStatsValue(StatsType.CursorRadius);
        RadiusNode.Scale = Vector3.One * Radius;

        FocusTickTime = 0.25f;
        FocusValue = StatsController.Instance.GetCurrentStatsValue(StatsType.CursorStartValue);
        FocusTickAmount = StatsController.Instance.GetCurrentStatsValue(StatsType.CursorTickAmount);
        FocusTickDecay = StatsController.Instance.GetCurrentStatsValue(StatsType.CursorTickDecay);
        MoveSpeed = StatsController.Instance.GetCurrentStatsValue(StatsType.CursorMoveSpeed);
        MoveFocusSpeed = MoveSpeed * StatsController.Instance.GetCurrentStatsValue(StatsType.CursorMoveSpeedMultiplierDuringFocus);
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
    }

    private void PhysicsProcess_MoveCursor(float delta)
    {
        if (!InputEnabled) return;

        GlobalPosition += DesiredVelocity * MoveSpeed;
    }

    private void Process_Target()
    {
        if (!InputEnabled) return;
        if (Filled) return;
        if (Empty) return;

        if (GameTime.Time < next_tick) return;
        next_tick = GameTime.Time + FocusTickTime;

        if (IsTargetInRange)
        {
            SetFocusValue(FocusValue + FocusTickAmount);
            GameView.Instance.FocusGained(FocusValue);
            Animation.Play("BounceIn");
            OnFocusTarget?.Invoke();
        }
        else
        {
            SetFocusValue(FocusValue - FocusTickDecay);
            GameView.Instance.FocusLost(FocusValue);
        }
    }

    private void SetFocusValue(float value)
    {
        FocusValue = Mathf.Clamp(value, 0f, 1f);

        if (FocusValue >= 1)
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
}
