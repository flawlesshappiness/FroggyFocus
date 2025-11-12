using Godot;

public partial class CrystalElevator : Node3D
{
    [Export]
    public AnimationPlayer AnimationPlayer;

    [Export]
    public Node3D MiddleNode;

    [Export]
    public Area3D PlatformArea;

    [Export]
    public CrystalEnergyContainer EnergyContainer;

    private bool is_powered;
    private bool is_up;
    private bool player_on_platform;

    public override void _Ready()
    {
        base._Ready();
        PlatformArea.BodyEntered += PlatformArea_PlayerEntered;
        PlatformArea.BodyExited += PlatformArea_PlayerExited;

        EnergyContainer.OnCompleted += EnergyContainerCompleted;
        EnergyContainer.OnNotCompleted += EnergyContainerNotCompleted;

        is_powered = EnergyContainer.IsCompleted;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (!is_powered) return;

        var player_is_up = Player.Instance.GlobalPosition.Y > MiddleNode.GlobalPosition.Y;
        var player_is_platform = player_on_platform && !Player.Instance.IsJumping;

        if (player_is_platform)
        {
            SetUp(true);
        }
        else if (is_up != player_is_up)
        {
            SetUp(!is_up);
        }
    }

    private void SetUp(bool up)
    {
        if (is_up == up) return;
        is_up = up;

        var anim = up ? "move_up" : "move_down";
        AnimationPlayer.Play(anim);
    }

    private void PlatformArea_PlayerEntered(GodotObject go)
    {
        player_on_platform = true;
    }

    private void PlatformArea_PlayerExited(GodotObject go)
    {
        player_on_platform = false;
    }

    private void EnergyContainerCompleted()
    {
        is_powered = true;
    }

    private void EnergyContainerNotCompleted()
    {
        is_powered = false;
    }
}
