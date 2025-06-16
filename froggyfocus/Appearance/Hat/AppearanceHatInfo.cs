using Godot;

[GlobalClass]
public partial class AppearanceHatInfo : Resource
{
    [Export]
    public AppearanceHatType Type;

    [Export]
    public PackedScene Prefab;
}
