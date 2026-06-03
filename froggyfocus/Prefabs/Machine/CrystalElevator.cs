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
        is_powered = EnergyContainer.IsCompleted;

        PlatformArea.BodyEntered += PlatformArea_PlayerEntered;
        PlatformArea.BodyExited += PlatformArea_PlayerExited;
        GameFlagsController.Instance.OnFlagChanged += Flag_Changed;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        GameFlagsController.Instance.OnFlagChanged -= Flag_Changed;
    }

    private void Flag_Changed(string id, int i)
    {
        if (id == EnergyContainer.HandInInfo.Id)
        {
            is_powered = EnergyContainer.IsCompleted;
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        var player_is_up = Player.Instance.GlobalPosition.Y > MiddleNode.GlobalPosition.Y;
        var player_is_platform = player_on_platform && !Player.Instance.Controller.IsJumping;

        SetUp(is_powered && (player_is_up || player_is_platform));
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
}
