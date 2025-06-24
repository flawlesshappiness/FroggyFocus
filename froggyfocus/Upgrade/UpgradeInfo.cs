using Godot;
using Godot.Collections;

[GlobalClass]
public partial class UpgradeInfo : Resource
{
    [Export]
    public StatsType Type;

    [Export]
    public Array<float> Values;

    [Export]
    public Array<int> OverridePrice;
}
