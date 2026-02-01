using Godot;
using System;

public partial class TopDownController : CharacterBody3D
{
    [Export]
    public Camera3D Camera;

    public Camera3D OverrideCamera { get; set; }
    public Camera3D CurrentCamera => OverrideCamera ?? Camera;

    public static MultiLock GravityLock = new();
    public Vector3 DesiredMoveVelocity { get; private set; }
    public Vector3 DesiredJumpVelocity { get; private set; }
    private float Gravity => GravityLock.IsLocked ? 0 : 20;

    public bool IsJumping => _jumping;
    public bool IsMoving => _moving;

    private bool _moving;
    private bool _jumping;
    private float _gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

    public Action OnJump, OnLand;
    public Action OnMoveStart, OnMoveStop;

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        var fdelta = Convert.ToSingle(delta);
        PhysicsProcess_Move(fdelta);
    }

    private void PhysicsProcess_Move(float delta)
    {
        Vector3 velocity = Velocity;
        var grounded = IsOnFloor();

        if (grounded)
        {
            if (DesiredMoveVelocity.Length() >= 0.01f)
            {
                velocity.X = DesiredMoveVelocity.X;
                velocity.Z = DesiredMoveVelocity.Z;
            }
            else
            {
                var decel = 15 * (float)delta;
                velocity.X = Mathf.MoveToward(Velocity.X, 0, decel);
                velocity.Z = Mathf.MoveToward(Velocity.Z, 0, decel);
            }

            if (DesiredJumpVelocity.Length() >= 0.01f)
            {
                velocity = DesiredJumpVelocity;
            }
        }
        else
        {
            if (IsJumping) // Reapply horizontal jump velocity
            {
                velocity = DesiredJumpVelocity.Set(y: velocity.Y);
            }

            velocity.Y -= Gravity * (float)delta;
        }

        Velocity = velocity;
        MoveAndSlide();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        Process_States();
    }

    private void Process_States()
    {
        var grounded = IsOnFloor();

        var has_jump_velocity = Velocity.Y > 0.01f;
        if (!_jumping && has_jump_velocity)
        {
            _jumping = true;
            OnJump?.Invoke();
        }
        else if (_jumping && grounded)
        {
            _jumping = false;
            Velocity = Vector3.Zero;
            DesiredJumpVelocity = Vector3.Zero;
            OnLand?.Invoke();
        }

        var has_move_velocity = Velocity.Length() >= 0.01f;
        if (!_moving && has_move_velocity)
        {
            _moving = true;
            OnMoveStart?.Invoke();
        }
        else if (_moving && !has_move_velocity)
        {
            _moving = false;
            OnMoveStop?.Invoke();
        }
    }

    protected void Move(Vector2 input, float speed)
    {
        if (input.Length() > 0)
        {
            Vector3 direction = CurrentCamera.GlobalBasis * (new Vector3(input.X, 0, input.Y)).Normalized();
            Move(direction * speed);
        }
        else
        {
            Stop();
        }
    }

    protected void Move(Vector3 velocity)
    {
        DesiredMoveVelocity = velocity;
    }

    protected void Stop()
    {
        DesiredMoveVelocity = Vector3.Zero;
    }

    protected void Jump(Vector3 velocity)
    {
        DesiredJumpVelocity = velocity;
    }
}
