using Godot;

[GlobalClass]
public partial class UpgradeInfo : Resource
{
    [Export]
    public StatsType Type;

    [Export]
    public int Level;

    [Export]
    public float Value;

    [Export]
    public int Price;
}
