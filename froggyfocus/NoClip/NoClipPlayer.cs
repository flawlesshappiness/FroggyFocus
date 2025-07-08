using Godot;
using System;

public partial class NoClipPlayer : Node3D
{
    [Export]
    public float MoveSpeed;

    [Export]
    public Camera3D Camera;

    public bool Enabled { get; set; }

    private Vector3 desired_velocity;

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (!Enabled) return;
        Process_Move();
    }

    private void Process_Move()
    {
        var input = PlayerInput.GetMoveInput();
        if (input.Length() > 0)
        {
            Vector3 direction = Camera.GlobalBasis * (new Vector3(input.X, 0, input.Y)).Normalized();
            var mul = PlayerInput.Jump.Held ? 2f : 1f;
            desired_velocity = direction * MoveSpeed * mul;
        }
        else
        {
            desired_velocity = Vector3.Zero;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        var fdelta = Convert.ToSingle(delta);
        PhysicsProcess_Move(fdelta);
    }

    private void PhysicsProcess_Move(float delta)
    {
        if (!Enabled) return;

        if (desired_velocity.Length() >= 0.01f)
        {
            GlobalPosition += desired_velocity * delta;
        }
    }

    public override void _Input(InputEvent e)
    {
        base._Input(e);
        if (!Enabled) return;
        Input_Rotation(e);

        if (PlayerInput.Interact.Pressed)
        {
            NoClipController.Instance.StopNoclip();
        }
    }

    private void Input_Rotation(InputEvent e)
    {
        var sensitivity = 0.003f;
        float tilt_limit = Mathf.DegToRad(50f);

        if (e is InputEventMouseMotion mouse_motion)
        {
            var relative = mouse_motion.Relative;
            var x = Mathf.Clamp(Rotation.X - relative.Y * sensitivity, -tilt_limit, tilt_limit);
            var y = Rotation.Y - relative.X * sensitivity;
            Rotation = new Vector3(x, y, Rotation.Z);
        }
    }
}
