using Godot;

public partial class Character : Node3D
{
    private Vector3? _facing_direction;

    public override void _Process(double delta)
    {
        base._Process(delta);
        Process_FacingDirection();
    }

    private void Process_FacingDirection()
    {
        if (_facing_direction == null) return;
        RotateToDirection(_facing_direction ?? Vector3.Forward);
    }

    public void StartFacingPosition(Vector3 position)
    {
        StartFacingDirection(GlobalPosition.DirectionTo(position));
    }

    public void StartFacingDirection(Vector3 direction)
    {
        _facing_direction = direction;
    }

    public void StopFacingDirection()
    {
        _facing_direction = null;
    }

    private void RotateToDirection(Vector3 direction)
    {
        var rotation_speed = 10f;
        var ry = Mathf.LerpAngle(Rotation.Y, Mathf.Atan2(-direction.X, -direction.Z), rotation_speed * GameTime.DeltaTime);
        Rotation = new Vector3(0, ry, 0);
    }
}
