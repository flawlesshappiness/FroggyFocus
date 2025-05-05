using Godot;

public partial class Player : TopDownController
{
    [Export]
    public float MoveSpeed;

    [Export]
    public PlayerInteract PlayerInteract;

    [Export]
    public FrogCharacter Character;

    [Export]
    public Godot.Curve JumpLengthCurve;

    [Export]
    public Godot.Curve JumpHeightCurve;

    public static Player Instance { get; private set; }

    public static MultiLock MovementLock = new();
    public static MultiLock InteractLock = new();

    private float jump_charge;

    public override void _Ready()
    {
        base._Ready();
        Instance = this;
        SetCameraTarget();

        OnMoveStart += () => MoveChanged(true);
        OnMoveStop += () => MoveChanged(false);
        OnJump += () => JumpChanged(true);
        OnLand += () => JumpChanged(false);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        Process_Facing();
        Process_Move();
        Process_Interact();
        Process_Jump();
        Process_CameraOffset();
    }

    private void Process_Move()
    {
        var input = PlayerInput.GetMoveInput();
        if (input.Length() > 0 && !MovementLock.IsLocked && !PlayerInput.Jump.Held)
        {
            Move(input, MoveSpeed);
        }
        else
        {
            Stop();
        }
    }

    private void Process_Facing()
    {
        if (MovementLock.IsLocked) return;
        if (IsJumping) return;

        var input = PlayerInput.GetMoveInput();
        if (input.Length() < 0.1f) return;

        Character.StartFacingDirection(new Vector3(input.X, 0, input.Y));
    }

    private void Process_Jump()
    {
        if (MovementLock.IsLocked) return;

        if (PlayerInput.Jump.Held)
        {
            jump_charge += GameTime.DeltaTime * 0.5f;
            Character.SetCharging(true);
        }
        else if (PlayerInput.Jump.Released)
        {
            var t = Mathf.Clamp(jump_charge, 0, 1);
            jump_charge = 0;
            var height = JumpHeightCurve.Sample(t);
            var length = JumpLengthCurve.Sample(t);
            var velocity = Character.Basis * new Vector3(0, height, -length);
            Jump(velocity);
            Character.SetCharging(false);
        }
    }

    private void Process_Interact()
    {
        if (PlayerInput.Interact.Released)
        {
            Interact();
        }
    }

    private void Interact()
    {
        if (InteractLock.IsLocked) return;

        PlayerInteract.TryInteract();
    }

    public void SetCameraTarget()
    {
        CameraController.Instance.Speed = 1.0f;
        CameraController.Instance.Target = this;
        CameraController.Instance.TargetRotation = new Vector3(-60, 0, 0);
    }

    private void Process_CameraOffset()
    {
        if (CameraController.Instance.Target != this) return;

        var default_offset = new Vector3(0, 5, 3f);
        var direction_offset = Character.Basis * Vector3.Forward * 2;
        var offset = default_offset + direction_offset;
        CameraController.Instance.Offset = offset;
    }

    public static void SetAllLocks(string key, bool locked)
    {
        MovementLock.SetLock(key, locked);
        InteractLock.SetLock(key, locked);
    }

    private void MoveChanged(bool moving)
    {
        Character.SetMoving(moving);
    }

    private void JumpChanged(bool jumping)
    {
        Character.SetJumping(jumping);

        CameraController.Instance.Speed = jumping ? 4.0f : 1.0f;
    }
}
