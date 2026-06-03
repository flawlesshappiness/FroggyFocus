using Godot;

public partial class CrystalCrane : Node3D
{
    [Export]
    public CrystalEnergyContainer EnergyContainer;

    [Export]
    public AnimationPlayer AnimationPlayer;

    private bool is_up;

    public override void _Ready()
    {
        base._Ready();
        SetPowered(EnergyContainer.IsCompleted);

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
            SetPowered(EnergyContainer.IsCompleted);
        }
    }

    private void SetPowered(bool up)
    {
        if (is_up == up) return;
        is_up = up;

        var anim = up ? "rotate_on" : "rotate_off";
        AnimationPlayer.Play(anim);
    }
}
