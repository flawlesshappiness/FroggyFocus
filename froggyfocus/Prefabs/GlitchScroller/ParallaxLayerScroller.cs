using Godot;

public partial class ParallaxLayerScroller : Node2D
{
    [Export]
    public float Z;

    public void Process_Parallax(Camera2D camera)
    {
        var x = camera.Position.X * Z;
        Position = Position.Set(x: x);
    }
}
