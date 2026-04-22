using Godot;

public partial class FocusFrog : Node3D
{
    [Export]
    public CuteFrogCharacter Character;

    private Node3D Target { get; set; }

    private float angle_big;
    private float angle_small;
    private float turn_cooldown;

    public override void _Ready()
    {
        base._Ready();
        angle_big = Mathf.DegToRad(20f);
        angle_small = Mathf.DegToRad(5f);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        Process_Angle();
    }

    private void Process_Angle()
    {
        if (Target == null) return;

        var angle = GetAngleToTarget();
        var is_big = Mathf.Abs(angle) > angle_big;

        if (is_big)
        {
            TurnToTarget();
        }
    }

    public void SetTarget(Node3D target)
    {
        Target = target;
        TurnToTarget();
    }

    public void ClearTarget()
    {
        Target = null;
    }

    public void TurnToTarget(bool with_cooldown = true)
    {
        if (Target == null) return;
        if (GameTime.Time < turn_cooldown && with_cooldown) return;

        turn_cooldown = GameTime.Time + 1f;

        var angle = GetAngleToTarget();
        var is_small = Mathf.Abs(angle) < angle_small;
        var is_right = angle < 0;

        Character.StartFacingPosition(Target.GlobalPosition);

        if (is_small)
        {
            // Do nothing
        }
        else if (is_right)
        {
            Character.SetTurnRight();
        }
        else
        {
            Character.SetTurnLeft();
        }
    }

    private float GetAngleToTarget()
    {
        if (Target == null) return 0;

        var forward = Character.Basis * Vector3.Forward;
        var direction = GlobalPosition.DirectionTo(Target.GlobalPosition);
        var angle = forward.SignedAngleTo(direction, Vector3.Up);
        return angle;
    }
}
