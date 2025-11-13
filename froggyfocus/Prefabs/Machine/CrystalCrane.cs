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
        var is_powered = EnergyContainer == null || EnergyContainer.IsCompleted;
        SetPowered(is_powered);

        if (EnergyContainer != null)
        {
            EnergyContainer.OnCompleted += EnergyContainerCompleted;
            EnergyContainer.OnNotCompleted += EnergyContainerNotCompleted;
        }
    }

    private void SetPowered(bool up)
    {
        if (is_up == up) return;
        is_up = up;

        var anim = up ? "rotate_on" : "rotate_off";
        AnimationPlayer.Play(anim);
    }

    private void EnergyContainerCompleted()
    {
        SetPowered(true);
    }

    private void EnergyContainerNotCompleted()
    {
        SetPowered(false);
    }
}
