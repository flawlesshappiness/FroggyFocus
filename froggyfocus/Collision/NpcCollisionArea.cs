using Godot;
using System;

public partial class NpcCollisionArea : Area3D
{
    [Export]
    public float PushVelocity = 10f;

    private float radius;
    private bool has_player;

    public override void _Ready()
    {
        base._Ready();
        InitializeRadius();
        BodyEntered += _BodyEntered;
        BodyExited += _BodyExited;
    }

    private void InitializeRadius()
    {
        var cshape = this.GetNodeInChildren<CollisionShape3D>();
        var shape = cshape.Shape;

        if (shape is CylinderShape3D cylinder)
        {
            radius = cylinder.Radius;
        }
        else if (shape is BoxShape3D box)
        {
            radius = Mathf.Min(box.Size.X, box.Size.Z) * 0.5f;
        }
        else if (shape is CapsuleShape3D capsule)
        {
            radius = capsule.Radius;
        }
        else if (shape is SphereShape3D sphere)
        {
            radius = sphere.Radius;
        }
    }

    private void _BodyEntered(GodotObject go)
    {
        has_player = true;
    }

    private void _BodyExited(GodotObject go)
    {
        has_player = false;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        var fdelta = Convert.ToSingle(delta);
        Process_PushPlayer(fdelta);
    }

    private void Process_PushPlayer(float delta)
    {
        if (!has_player) return;
        var dir = (Player.Instance.GlobalPosition - GlobalPosition).Set(y: 0);
        var length = dir.Length();
        var t_radius = 1f - Mathf.Clamp(length / radius, 0f, 1f);
        var velocity = dir.Normalized() * PushVelocity * delta * t_radius;
        Player.Instance.GlobalPosition += velocity;
    }
}
