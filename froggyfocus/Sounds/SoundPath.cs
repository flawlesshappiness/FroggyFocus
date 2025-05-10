using Godot;
using Godot.Collections;

public partial class SoundPath : Node3D
{
    [Export]
    public Array<Path3D> Paths;

    private Vector3 target_position;

    public override void _Process(double delta)
    {
        base._Process(delta);
        Process_TargetPosition();
        Process_Move();
    }

    private void Process_Move()
    {
        var speed = 2f;
        GlobalPosition = GlobalPosition.Lerp(target_position, speed * GameTime.DeltaTime);
    }

    private void Process_TargetPosition()
    {
        target_position = GetClosestPosition() ?? GlobalPosition;
    }

    private Vector3? GetClosestPosition()
    {
        var dist = float.MaxValue;
        Vector3? closest = null;

        var global_player = Player.Instance.GlobalPosition;
        foreach (var path in Paths)
        {
            var global_path = path.GlobalPosition;
            var local_target = global_player - global_path;
            var global_point = global_path + path.Curve.GetClosestPoint(local_target);
            var d = global_point.DistanceTo(global_player);

            if (d < dist)
            {
                closest = global_point;
                dist = d;
            }
        }

        return closest;
    }
}
