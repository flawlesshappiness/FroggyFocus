using Godot;
using Godot.Collections;

[GlobalClass]
public partial class FocusCharacterInfo : Resource
{
    [Export]
    public PackedScene Scene;

    [Export]
    public string Name;

    [Export(PropertyHint.MultilineText)]
    public string LocationHint;

    [Export]
    public string Accessory;

    [Export]
    public float FocusValue = 50;

    [Export]
    public float MoneyMultiplier = 1f;

    [Export]
    public Vector2 MoveSpeedRange = new Vector2(1, 1);

    [Export]
    public Vector2 MoveLengthRange = new Vector2(1, 1);

    [Export]
    public Vector2 MoveDelayRange = new Vector2(1, 1);

    [Export]
    public Array<FocusSkillCheckType> SkillChecks;

    [Export]
    public Array<FocusCharacterTag> Tags;
}
