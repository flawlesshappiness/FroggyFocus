using Godot;
using Godot.Collections;

[GlobalClass]
public partial class FocusCharacterInfo : Resource
{
    [Export]
    public PackedScene Scene;

    [Export]
    public string Name;

    [Export]
    public string Variation;

    [Export(PropertyHint.MultilineText)]
    public string LocationHint;

    [Export]
    public string Accessory;

    [Export]
    public float FocusValueOverride = 0;

    [Export]
    public float MoneyMultiplier = 1f;

    [Export]
    public FocusCharacterMoveType MoveType = FocusCharacterMoveType.Walk;

    [Export]
    public FocusCharacterSpeedType SpeedType = FocusCharacterSpeedType.Normal;

    [Export]
    public FocusCharacterDistanceType DistanceType = FocusCharacterDistanceType.Normal;

    [Export]
    public FocusCharacterDelayType DelayType = FocusCharacterDelayType.Normal;

    [Export]
    public float OverrideSize;

    [Export]
    public int OverrideRarity;

    [Export]
    public Array<FocusAttackType> Attacks;

    [Export]
    public Array<FocusCharacterTag> Tags;

    [Export]
    public bool ExcludeFromBestiary;

    [Export]
    public Vector3 PreviewOffset;
}
