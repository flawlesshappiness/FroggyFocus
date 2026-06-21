using Godot;
using System.Linq;

public partial class PlayerStableGroundController : Node3D
{
    [Export]
    public FrogPlayerController Controller;

    public bool IsOnStableGround { get; private set; }
    public Vector3 LastPosition { get; private set; }
    private bool IsFirstFrames => frame < 10;

    private int frame;

    public override void _Ready()
    {
        base._Ready();
        Controller.OnLand += Controller_Land;
    }

    public void InitializePosition()
    {
        LastPosition = GetClosestPosition();
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        if (IsFirstFrames)
        {
            frame++;
        }
    }

    private void Controller_Land()
    {
        if (IsFirstFrames) return;
        EvaluateStableGround();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        Process_RespawnPosition();
    }

    private void Process_RespawnPosition()
    {
        if (!IsOnStableGround) return;

        if (Controller.IsIdle || Controller.IsMoving || Controller.IsCharging)
        {
            LastPosition = GlobalPosition;
        }
    }


    private void EvaluateStableGround()
    {
        var closest_position = GetClosestPosition();
        IsOnStableGround = GlobalPosition.DistanceTo(closest_position) < 0.5f;
    }

    private Vector3 GetClosestPosition()
    {
        return NavigationServer3D.MapGetClosestPoint(NavigationServer3D.GetMaps().First(), GlobalPosition);
    }
}
