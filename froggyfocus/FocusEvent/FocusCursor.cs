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
    private FocusEventAxis Axis { get; set; }
    private Vector3 DesiredVelocity { get; set; }
    private float Radius { get; set; }
    private float DistanceToTarget => Target == null ? 0f : GlobalPosition.DistanceTo(Target.GlobalPosition) - Target.Radius;
    private bool IsTargetInRange => DistanceToTarget < Radius;
    private bool Focusing => PlayerInput.Interact.Held;
    private float FocusValue { get; set; }
    private float FocusTickTime { get; set; }
    private float FocusTickAmount { get; set; }
    private float FocusTickDecay { get; set; }
    private float MoveSpeed { get; set; }
    private float MoveFocusSpeed { get; set; }
    private bool Filled { get; set; }
    private bool Empty { get; set; }

    private bool is_focusing;
    private bool is_focusing_on_target;
    private float next_tick;

    public event Action OnFocusStarted;
    public event Action OnFocusStopped;
    public event Action OnFocusTarget;
    public event Action OnFocusTargetEnter;
    public event Action OnFocusTargetExit;
    public event Action OnFocusFilled;
    public event Action OnFocusEmpty;

    public void Initialize(FocusTarget target, FocusEventAxis axis)
    {
        Target = target;
        Axis = axis;
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
        // TODO: Load data
        Radius = 0.4f;
        RadiusNode.Scale = Vector3.One * Radius;

        FocusValue = 0.25f;
        FocusTickTime = 0.25f;
        FocusTickAmount = 0.03f;
        FocusTickDecay = 0.01f;
        MoveSpeed = 0.02f;
        MoveFocusSpeed = MoveSpeed * 0.1f;
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (Focusing && !is_focusing)
        {
            is_focusing = true;
            OnFocusStarted?.Invoke();
        }
        else if (!Focusing && is_focusing)
        {
            is_focusing = false;
            OnFocusStopped?.Invoke();
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        var fdelta = Convert.ToSingle(delta);
        Process_Input();
        Process_Target();
        Process_Graphic();
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
        DesiredVelocity = GetInputVelocity(input);
    }

    private Vector3 GetInputVelocity(Vector2 input) => Axis switch
    {
        FocusEventAxis.XZ => new Vector3(input.X, 0, input.Y),
        FocusEventAxis.XY => new Vector3(input.X, -input.Y, 0),
        FocusEventAxis.X => new Vector3(input.X, 0, 0),
        _ => Vector3.Zero
    };

    private void PhysicsProcess_MoveCursor(float delta)
    {
        if (!InputEnabled) return;

        var speed = Focusing ? MoveFocusSpeed : MoveSpeed;
        GlobalPosition += DesiredVelocity * speed;
    }

    private void Process_Target()
    {
        if (!InputEnabled) return;
        if (Filled) return;
        if (Empty) return;

        if (GameTime.Time < next_tick) return;
        next_tick = GameTime.Time + FocusTickTime;

        if (IsTargetInRange && Focusing)
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

    private void Process_Graphic()
    {
        if (IsTargetInRange && Focusing)
        {
            if (!is_focusing_on_target)
            {
                is_focusing_on_target = true;
                OnFocusTargetEnter?.Invoke();
            }
        }
        else
        {
            if (is_focusing_on_target)
            {
                is_focusing_on_target = false;
                OnFocusTargetExit?.Invoke();
            }
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
