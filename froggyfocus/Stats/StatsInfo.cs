using Godot;

[GlobalClass]
public partial class StatsInfo : Resource
{
    [Export]
    public StatsType Type;

    [Export]
    public string Name;

    [Export]
    public string Description;
}
