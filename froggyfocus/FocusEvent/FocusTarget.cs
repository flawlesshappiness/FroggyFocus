using Godot;

public partial class FocusTarget : Node3D
{
    [Export]
    public Vector2 SizeRange = new Vector2(1, 1);

    public float Size { get; set; }
    public float Radius => Size * 0.5f;

    public override void _Ready()
    {
        base._Ready();

        var rng = new RandomNumberGenerator();
        var size = rng.RandfRange(SizeRange.X, SizeRange.Y);
        SetSize(size);
    }

    public void SetSize(float size)
    {
        Size = size;
        Scale = Vector3.One * size;
    }
}
