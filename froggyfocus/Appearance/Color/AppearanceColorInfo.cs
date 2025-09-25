using Godot;

[GlobalClass]
public partial class AppearanceColorInfo : Resource
{
    [Export]
    public ItemCategory Category;

    [Export]
    public ItemType Type;

    [Export]
    public Color Color = Colors.White;
}
