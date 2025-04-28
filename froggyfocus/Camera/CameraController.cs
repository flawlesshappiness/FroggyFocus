using Godot;
using System;

public partial class CameraController : Camera3D
{
    [Export]
    public Node3D Target;

    [Export]
    public float Speed;

    [Export]
    public Vector3 Offset;

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        var fdelta = Convert.ToSingle(delta);
        PhysicsProcess_Follow(fdelta);
    }

    private void PhysicsProcess_Follow(float delta)
    {
        var target_position = Target.GlobalPosition.Set(y: GlobalPosition.Y) + Offset;
        GlobalPosition = GlobalPosition.Lerp(target_position, Speed * delta);
    }
}
