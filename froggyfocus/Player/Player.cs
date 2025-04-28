using Godot;

public partial class Player : TopDownController
{
    [Export]
    public float MoveSpeed;

    public static MultiLock MovementLock = new();

    public override void _Process(double delta)
    {
        base._Process(delta);
        Process_Move();
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
}
