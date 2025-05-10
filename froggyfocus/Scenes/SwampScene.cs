using Godot;

public partial class SwampScene : GameScene
{
    [Export]
    public Player Player;


    public override void _Process(double delta)
    {
        base._Process(delta);
        Process_PlayerFall();
    }

    private void Process_PlayerFall()
    {
        if (Player.GlobalPosition.Y < -10)
        {
            Player.Respawn();
            CameraController.Instance.TeleportCameraToTarget();
        }
    }
}
