using Godot;
using Godot.Collections;

[GlobalClass]
public partial class ObjectiveInfo : Resource
{
    [Export]
    public string Description;

    [Export]
    public FocusCharacterTag RequirementTag;

    [Export]
    public Array<int> Values;

    [Export]
    public Array<int> MoneyRewards;
}
