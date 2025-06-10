using Godot;
using System;

public partial class NoiseOscillator : Node3D
{
    [Export]
    public FastNoiseLite Noise;

    [Export]
    public Vector3 Velocity;

    [Export]
    public Godot.Curve CurveX;

    [Export]
    public Godot.Curve CurveY;

    private Vector3 t_offset;

    public override void _Process(double delta)
    {
        base._Process(delta);
        var fdelta = Convert.ToSingle(delta);

        var x = (t_offset.X + Velocity.X * fdelta) % 1f;
        var y = (t_offset.Y + Velocity.Y * fdelta) % 1f;

        t_offset = new Vector3(x, y, 0);

        var x_offset = CurveX.Sample(t_offset.X);
        var y_offset = CurveY.Sample(t_offset.Y);

        var offset = new Vector3(x_offset, y_offset, 0);

        Noise.Offset = offset;
    }
}
