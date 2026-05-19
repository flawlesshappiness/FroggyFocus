using Godot;
using System;

[GlobalClass]
public partial class TriggerArea2D : Area2D
{
    public event Action<Node2D> OnNodeEntered;
    public event Action<Node2D> OnNodeExited;

    public override void _Ready()
    {
        base._Ready();
        BodyEntered += _BodyEntered;
        BodyExited += _BodyExited;
    }

    private void _BodyEntered(GodotObject go)
    {
        var node = go as Node2D;
        if (node == null) return;

        OnNodeEntered?.Invoke(node);
    }

    private void _BodyExited(GodotObject go)
    {
        var node = go as Node2D;
        if (node == null) return;

        OnNodeExited?.Invoke(node);
    }
}
