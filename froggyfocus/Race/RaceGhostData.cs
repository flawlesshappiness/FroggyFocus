using Godot;
using System.Collections.Generic;
using System.Text.Json.Serialization;

public class RaceGhostData
{
    public List<RaceGhostSnapshot> Snapshots { get; set; } = new();
}

public class RaceGhostSnapshot
{
    public float Time { get; set; }
    public float PositionX { get; set; }
    public float PositionY { get; set; }
    public float PositionZ { get; set; }
    public float RotationX { get; set; }
    public float RotationY { get; set; }
    public float RotationZ { get; set; }
    public string Animation { get; set; }

    [JsonIgnore]
    public Vector3 Position => new Vector3(PositionX, PositionY, PositionZ);

    [JsonIgnore]
    public Vector3 Rotation => new Vector3(RotationX, RotationY, RotationZ);
}