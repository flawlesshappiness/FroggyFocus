using Godot;

[GlobalClass]
public partial class ShopItemInfo : Resource
{
    [Export]
    public ItemCategory Category;

    [Export]
    public ItemType Type;

    [Export]
    public int Price;
}
