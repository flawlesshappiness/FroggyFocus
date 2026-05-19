using Godot;
using System;

[GlobalClass]
public partial class TriggerArea3D : Area3D
{
    public event Action<Node3D> OnNodeEntered;
    public event Action<Node3D> OnNodeExited;

    public override void _Ready()
    {
        base._Ready();
        BodyEntered += _BodyEntered;
        BodyExited += _BodyExited;
    }

    private void _BodyEntered(GodotObject go)
    {
        var node = go as Node3D;
        if (node == null) return;

        OnNodeEntered?.Invoke(node);
    }

    private void _BodyExited(GodotObject go)
    {
        var node = go as Node3D;
        if (node == null) return;

        OnNodeExited?.Invoke(node);
    }
}
