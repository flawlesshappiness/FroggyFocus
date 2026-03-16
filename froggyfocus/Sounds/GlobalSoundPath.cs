using Godot;
using Godot.Collections;

public partial class GlobalSoundPath : AudioStreamPlayer
{
    [Export]
    public Vector2 DistanceRange;

    [Export]
    public float VolumeMax;

    [Export]
    public Array<Path3D> Paths;

    public override void _Process(double delta)
    {
        base._Process(delta);
        Process_Volume();
    }

    private void Process_Volume()
    {
        var position = GetClosestPosition() ?? Player.Instance.GlobalPosition;
        var distance = Player.Instance.GlobalPosition.DistanceTo(position);
        var t = Mathf.Clamp((distance - DistanceRange.X) / DistanceRange.Y, 0, 1);
        var volume = Mathf.Lerp(VolumeMax, 0, t);
        VolumeLinear = volume;
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
