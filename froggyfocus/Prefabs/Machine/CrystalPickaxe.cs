using Godot;

public partial class CrystalPickaxe : Node3D
{
    [Export]
    public CrystalEnergyContainer EnergyContainer;

    [Export]
    public AnimationPlayer AnimationPlayer;

    [Export]
    public Node3D MiddleNode;

    private bool is_powered;
    private bool is_up;

    public override void _Ready()
    {
        base._Ready();
        is_powered = EnergyContainer == null || EnergyContainer.IsCompleted;

        if (EnergyContainer != null)
        {
            EnergyContainer.OnCompleted += EnergyContainerCompleted;
            EnergyContainer.OnNotCompleted += EnergyContainerNotCompleted;
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (!is_powered) return;

        var player_is_up = Player.Instance.GlobalPosition.Y > MiddleNode.GlobalPosition.Y;
        SetUp(player_is_up);
    }

    private void SetUp(bool up)
    {
        if (is_up == up) return;
        is_up = up;

        var anim = up ? "move_up" : "move_down";
        AnimationPlayer.Play(anim);
    }

    private void EnergyContainerCompleted()
    {
        is_powered = true;
    }

    private void EnergyContainerNotCompleted()
    {
        is_powered = false;
        SetUp(false);
    }
}
