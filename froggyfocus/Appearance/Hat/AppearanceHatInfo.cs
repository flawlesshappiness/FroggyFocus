using Godot;

[GlobalClass]
public partial class AppearanceHatInfo : Resource
{
    [Export]
    public AppearanceHatType Type;

    [Export]
    public PackedScene Prefab;

    [Export]
    public string Name;

    [Export]
    public int Price;

    [Export]
    public bool Locked;
}
