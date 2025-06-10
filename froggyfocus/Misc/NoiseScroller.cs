using Godot;
using System;

public partial class NoiseScroller : Node3D
{
    [Export]
    public FastNoiseLite Noise;

    [Export]
    public Vector3 Velocity;

    public override void _Process(double delta)
    {
        base._Process(delta);
        var fdelta = Convert.ToSingle(delta);
        Noise.Offset += Velocity * fdelta;
    }
}
