using Godot;
using Godot.Collections;

[GlobalClass]
public partial class UpgradeInfo : Resource
{
    [Export]
    public UpgradeType Type;

    [Export]
    public string Name;

    [Export]
    public string Description;

    [Export]
    public Array<float> Values;

    [Export]
    public Array<int> OverridePrice;
}
