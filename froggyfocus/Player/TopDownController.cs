using Godot;
using System;

public partial class TopDownController : CharacterBody3D
{
    public static MultiLock GravityLock = new();
    public Vector3 DesiredMoveVelocity { get; private set; }
    private float Gravity => GravityLock.IsLocked ? 0 : _gravity;

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
            if (_jumping)
            {
                _jumping = false;
                OnLand?.Invoke();
            }

            if (DesiredMoveVelocity != Vector3.Zero)
            {
                if (!_moving)
                {
                    _moving = true;
                    OnMoveStart?.Invoke();
                }

                velocity.X = DesiredMoveVelocity.X;
                velocity.Z = DesiredMoveVelocity.Z;
            }
            else
            {
                if (_moving)
                {
                    _moving = false;
                    OnMoveStop?.Invoke();
                }

                var decel = 15 * (float)delta;
                velocity.X = Mathf.MoveToward(Velocity.X, 0, decel);
                velocity.Z = Mathf.MoveToward(Velocity.Z, 0, decel);
            }

            /*
            if (DesiredJumpVelocity != Vector3.Zero)
            {
                velocity += DesiredJumpVelocity;
                _jumping = true;
                OnJump?.Invoke();
            }
            */
        }
        else
        {
            velocity.Y -= Gravity * (float)delta;
        }

        Velocity = velocity;
        MoveAndSlide();
    }

    protected void Move(Vector2 input, float speed)
    {
        if (input.Length() > 0)
        {
            //Vector3 direction = (NeckHorizontal.Basis * new Vector3(input.X, 0, input.Y)).Normalized();
            Vector3 direction = (new Vector3(input.X, 0, input.Y)).Normalized();
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
}
