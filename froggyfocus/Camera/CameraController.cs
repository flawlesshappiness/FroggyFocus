using Godot;
using System;

public partial class CameraController : Camera3D
{
    [Export]
    public float Speed;
    public Node3D Target { get; set; }
    public Vector3 Offset { get; set; }
    public Vector3 TargetRotation { get; set; }

    public static CameraController Instance { get; private set; }

    public override void _Ready()
    {
        base._Ready();
        Instance = this;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        var fdelta = Convert.ToSingle(delta);
        PhysicsProcess_Follow(fdelta);
    }

    private void PhysicsProcess_Follow(float delta)
    {
        if (Target == null) return;

        var target_position = Target.GlobalPosition + Offset;
        GlobalPosition = GlobalPosition.Lerp(target_position, Speed * delta);
        GlobalRotationDegrees = GlobalRotationDegrees.LerpEulerAngles(TargetRotation, Speed * delta);
    }
}
