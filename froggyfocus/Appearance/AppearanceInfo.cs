using Godot;

[GlobalClass]
public partial class AppearanceInfo : Resource
{
    [Export]
    public ItemCategory Category;

    [Export]
    public ItemType Type;

    [Export]
    public ItemType DefaultPrimaryColor;

    [Export]
    public ItemType DefaultSecondaryColor;

    [Export]
    public bool HasSecondaryColor;

    [Export]
    public PackedScene Prefab;
}
