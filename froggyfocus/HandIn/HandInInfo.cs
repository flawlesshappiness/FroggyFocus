using Godot;
using Godot.Collections;

[GlobalClass]
public partial class HandInInfo : Resource
{
    [Export]
    public string Id;

    [Export]
    public Array<FocusCharacterInfo> PossibleRequests;

    [Export]
    public Vector2I CountRange;

    [Export]
    public Vector2 RewardMultiplierRange;

    [Export]
    public AppearanceHatType HatUnlock;

    [Export]
    public float CooldownInSeconds;
}
