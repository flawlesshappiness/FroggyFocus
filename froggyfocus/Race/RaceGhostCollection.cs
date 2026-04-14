using Godot;

[GlobalClass]
public partial class RaceGhostCollection : ResourceCollection<RaceGhostInfo>
{
    [Export]
    public PackedScene RaceGhostPrefab;
}
