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
    public string Accessory;

    [Export]
    public float FocusValue = 50;

    [Export]
    public Vector2 SizeRange = new Vector2(1, 1);

    [Export]
    public float MoveSpeed = 1f;

    [Export]
    public Vector2 MoveLengthRange = new Vector2(1, 1);

    [Export]
    public Vector2 MoveDelayRange = new Vector2(1, 1);

    [Export]
    public int CurrencyReward = 1;

    [Export(PropertyHint.Range, "0,1,0.01")]
    public float Difficulty;

    [Export]
    public Array<FocusSkillCheckType> SkillChecks;
}
