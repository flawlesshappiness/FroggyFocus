using Godot;

public partial class FallingCamera : Node3D
{
    [Export]
    public Camera3D Camera;

    [Export]
    public Marker3D LookMarker;

    [Export]
    public Node3D Target;

    public override void _Process(double delta)
    {
        base._Process(delta);
        Process_Target();
    }

    private void Process_Target()
    {
        LookMarker.GlobalPosition = Target.GlobalPosition.Set(y: GlobalPosition.Y);
        LookMarker.LookAt(LookMarker.GlobalPosition + (Target.GlobalPosition - Camera.GlobalPosition), up: Vector3.Forward);
        Camera.GlobalTransform = Camera.GlobalTransform.InterpolateWith(LookMarker.GlobalTransform, 1f * GameTime.DeltaTime);
    }
}
