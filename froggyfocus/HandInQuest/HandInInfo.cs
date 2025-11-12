using Godot;
using Godot.Collections;

[GlobalClass]
public partial class HandInInfo : Resource
{
    [Export]
    public string Id;

    [Export]
    public bool RequestPreviewHidden;

    [Export]
    public Array<FocusCharacterInfo> PossibleRequests;

    [Export]
    public Vector2I CountRange;

    [Export]
    public bool UniqueRequests;

    [Export]
    public int ClaimCountToUnlock;

    [Export]
    public bool HasItemUnlock;

    [Export]
    public ItemType ItemUnlock;

    [Export]
    public float CooldownInSeconds;

    public HandInData Data => HandIn.GetOrCreateData(Id);
}
