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
        is_powered = EnergyContainer.IsCompleted;
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
        SetUp(player_is_up && is_powered);
    }

    private void SetUp(bool up)
    {
        if (is_up == up) return;
        is_up = up;

        var anim = up ? "move_up" : "move_down";
        AnimationPlayer.Play(anim);
    }

    public void Hit()
    {
        Player.Instance.ThirdPersonCamera.StartShake(new ThirdPersonCamera.ShakeSettings
        {
            FadeOutDuration = 3,
        });
    }
}
