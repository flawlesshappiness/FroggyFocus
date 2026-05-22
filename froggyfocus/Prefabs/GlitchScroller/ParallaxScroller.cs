using Godot;
using System.Collections.Generic;

public partial class ParallaxScroller : Node2D
{
    [Export]
    public Camera2D Camera;

    private List<ParallaxLayerScroller> layers = new();

    public override void _Ready()
    {
        base._Ready();
        layers = this.GetNodesInChildren<ParallaxLayerScroller>();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        foreach (var layer in layers)
        {
            layer.Process_Parallax(Camera);
        }
    }
}
