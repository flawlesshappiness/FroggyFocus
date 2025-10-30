using Godot;

public partial class GlitchScene : GameScene
{
    [Export]
    public Node3D MatrixLabelParent;

    [Export]
    public PackedScene MatrixLabelPrefab;

    public override void _Ready()
    {
        base._Ready();
        InitializeMatrixLabels();
    }

    private void InitializeMatrixLabels()
    {
        var rng = new RandomNumberGenerator();
        var count = 200;
        var extent = 1f;
        var radius_range = new Vector2(90, 150);
        var scale_range = new Vector2(0.01f, 0.1f);

        for (int i = 0; i < count; i++)
        {
            var radius = radius_range.Range(rng.Randf());
            var t_radius = (radius - radius_range.X) / (radius_range.Y - radius_range.X);
            var scale = scale_range.Range(t_radius);

            var label = MatrixLabelPrefab.Instantiate<Node3D>();
            label.SetParent(MatrixLabelParent);

            var x = rng.RandfRange(-extent, extent);
            var z = rng.RandfRange(-extent, extent);
            label.Position = new Vector3(x, 0, z).Normalized() * radius;

            label.Scale = Vector3.One * scale;
        }
    }
}
