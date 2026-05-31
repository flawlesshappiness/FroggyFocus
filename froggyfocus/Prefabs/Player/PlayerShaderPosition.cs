using Godot;

public partial class PlayerShaderPosition : Node3D
{
    [Export]
    public FrogPlayerController Controller;

    private const string IdGlobalPlayerPosition = "global_player_position";
    private Vector3 PlayerPosition => Controller.Body.GlobalPosition.Add(y: 0.5f);

    public override void _Ready()
    {
        base._Ready();
        Controller.OnLand += Controller_Land;
    }

    private void Controller_Land()
    {
        UpdateGlobalShaderMoveTime();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        Process_ShaderPosition();
    }

    private void Process_ShaderPosition()
    {
        RenderingServer.GlobalShaderParameterSet(IdGlobalPlayerPosition, PlayerPosition);

        if (Controller.IsMoving && !Controller.IsJumping)
        {
            UpdateGlobalShaderMoveTime();
        }
    }

    private void UpdateGlobalShaderMoveTime()
    {
        RenderingServer.GlobalShaderParameterSet(IdGlobalPlayerPosition, GameTime.Time);
    }
}
