using Godot;

[GlobalClass]
public partial class ItemInfo : Resource
{
    [Export]
    public ItemCategory Category;

    [Export]
    public ItemType Type;

    [Export]
    public string Name;
}
