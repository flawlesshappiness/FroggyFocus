using Godot;

public partial class Player : TopDownController
{
    [Export]
    public float MoveSpeed;

    [Export]
    public PlayerInteract PlayerInteract;

    public static Player Instance { get; private set; }

    public static MultiLock MovementLock = new();
    public static MultiLock InteractLock = new();

    public override void _Ready()
    {
        base._Ready();
        Instance = this;
        SetCameraTarget();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        Process_Move();
        Process_Interact();
    }

    private void Process_Move()
    {
        var input = PlayerInput.GetMoveInput();
        if (input.Length() > 0 && !MovementLock.IsLocked)
        {
            Move(input, MoveSpeed);
            RotateToDirection(-DesiredMoveVelocity);
        }
        else
        {
            Stop();
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
        CameraController.Instance.Target = this;
        CameraController.Instance.Offset = new Vector3(0, 5, 1.4f);
        CameraController.Instance.GlobalRotationDegrees = new Vector3(-70, 0, 0);
    }

    public static void SetAllLocks(string key, bool locked)
    {
        MovementLock.SetLock(key, locked);
        InteractLock.SetLock(key, locked);
    }
}
