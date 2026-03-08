using Godot;
using Godot.Collections;

[GlobalClass]
public partial class HandInInfo : Resource
{
    [Export]
    public string Id;

    [Export]
    public string Name;

    [Export]
    public ItemType ItemUnlock;

    [Export]
    public Array<HandInRequestInfo> Requests;

    public HandInData Data => HandIn.GetOrCreateData(Id);
    public bool HasItemUnlock => !Item.IsNoneType(ItemUnlock) && !Item.IsOwned(ItemUnlock);
}
