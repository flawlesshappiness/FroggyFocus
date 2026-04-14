using Godot;

[GlobalClass]
public partial class RaceGhostInfo : Resource
{
    [Export]
    public string Id;

    [Export(PropertyHint.File)]
    public string Path;
}
