using Godot;
using Godot.Collections;

[GlobalClass]
public partial class ObjectiveInfo : Resource
{
    [Export]
    public string Description;

    [Export]
    public bool UseTag;

    [Export]
    public FocusCharacterTag RequirementTag;

    [Export]
    public int MinimumStars;

    [Export]
    public Array<int> Values;

    [Export]
    public Array<int> MoneyRewards;

    [Export]
    public Array<ItemType> ItemRewards;
}
