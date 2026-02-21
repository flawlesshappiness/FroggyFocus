using Godot;

[GlobalClass]
public partial class AppearanceInfo : Resource
{
    [Export]
    public ItemCategory Category;

    [Export]
    public ItemType Type;

    [Export]
    public Color DefaultPrimaryColor;

    [Export]
    public Color DefaultSecondaryColor;

    [Export]
    public bool HasSecondaryColor;

    [Export]
    public Texture2D Texture;

    [Export]
    public PackedScene Prefab;
}
