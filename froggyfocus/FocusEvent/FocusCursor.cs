using Godot;
using System;

public partial class FocusCursor : Area3D
{
    [Export]
    public MeshInstance3D MeshInstance;

    [Export]
    public Label3D FillLabel;

    public bool InputEnabled { get; set; }
    public FocusTarget Target { get; set; }
    private Vector3 DesiredVelocity { get; set; }
    private float Radius { get; set; }
    private float DistanceToTarget => Target == null ? 0f : GlobalPosition.DistanceTo(Target.GlobalPosition) - Target.Radius;
    private bool IsTargetInRange => DistanceToTarget < Radius;
    private bool Focusing => PlayerInput.Interact.Held;
    private float FocusValue { get; set; }
    private float FocusSpeed { get; set; }
    private float DecaySpeed { get; set; }
    private float MoveSpeed { get; set; }
    private float MoveFocusSpeed { get; set; }
    private bool Filled { get; set; }

    public event Action OnFocusFilled;

    public void Initialize()
    {
        Load();
        Filled = false;
        SetFocusValue(0);
        this.Enable();
        InputEnabled = true;
    }

    public void Load()
    {
        // TODO: Load data
        Radius = 0.4f;
        var mesh = MeshInstance.Mesh as CylinderMesh;
        mesh.BottomRadius = Radius;
        mesh.TopRadius = Radius;

        FocusSpeed = 0.3f;
        DecaySpeed = 0.25f;
        MoveSpeed = 0.1f;
        MoveFocusSpeed = MoveSpeed * 0.25f;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        var fdelta = Convert.ToSingle(delta);
        Process_Input();
        Process_Target(fdelta);
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

        var speed = Focusing ? MoveFocusSpeed : MoveSpeed;
        GlobalPosition += DesiredVelocity * speed;
    }

    private void Process_Target(float delta)
    {
        if (!InputEnabled) return;
        if (Filled) return;

        if (IsTargetInRange && Focusing)
        {
            SetFocusValue(FocusValue + delta * FocusSpeed);
        }
        else
        {
            SetFocusValue(FocusValue - delta * DecaySpeed);
        }
    }

    private void SetFocusValue(float value)
    {
        FocusValue = Mathf.Clamp(value, 0f, 1f);
        FillLabel.Text = $"{(int)(FocusValue * 100)}%";

        if (FocusValue >= 1)
        {
            Filled = true;
            OnFocusFilled?.Invoke();
        }
    }
}
