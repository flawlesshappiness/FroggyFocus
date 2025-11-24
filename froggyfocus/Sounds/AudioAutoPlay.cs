using Godot;

public partial class AudioAutoPlay : AudioStreamPlayer3D
{
    [Export]
    public bool RandomStart;

    public override void _Ready()
    {
        base._Ready();
        var rng = new RandomNumberGenerator();
        var position = RandomStart ? rng.RandfRange(0, (float)Stream.GetLength()) : 0;
        Play(position);
    }
}
