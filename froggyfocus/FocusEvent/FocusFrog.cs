using Godot;

public partial class FocusFrog : Node3D
{
    [Export]
    public CuteFrogCharacter CuteFrogCharacter;

    [Export]
    public PixelFrogCharacter PixelFrogCharacter;

    private FrogCharacter Frog { get; set; }
    private Node3D Target { get; set; }

    private float angle_must_turn;
    private float angle_small;
    private float angle_big;
    private float turn_cooldown;

    public override void _Ready()
    {
        base._Ready();
        angle_must_turn = Mathf.DegToRad(20f);
        angle_small = Mathf.DegToRad(5f);
        angle_big = Mathf.DegToRad(100f);
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
        var is_big = Mathf.Abs(angle) > angle_must_turn;

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

    public void TurnToTarget()
    {
        if (Target == null) return;
        if (GameTime.Time < turn_cooldown) return;

        turn_cooldown = GameTime.Time + 1f;

        var angle = GetAngleToTarget();
        var is_small = Mathf.Abs(angle) < angle_small;
        var is_big = Mathf.Abs(angle) > angle_big;
        var is_right = angle < 0;

        CuteFrogCharacter.StartFacingPosition(Target.GlobalPosition);
        PixelFrogCharacter.RotateToDirectionImmediate(GlobalPosition.DirectionTo(Target.GlobalPosition));

        if (is_small)
        {
            // Do nothing
        }
        else if (is_big)
        {
            CuteFrogCharacter.SetJumpQuick();
        }
        else if (is_right)
        {
            CuteFrogCharacter.SetTurnRight();
        }
        else
        {
            CuteFrogCharacter.SetTurnLeft();
        }
    }

    private float GetAngleToTarget()
    {
        if (Target == null) return 0;

        var forward = CuteFrogCharacter.Basis * Vector3.Forward;
        var direction = GlobalPosition.DirectionTo(Target.GlobalPosition.Set(y: GlobalPosition.Y));
        var angle = forward.SignedAngleTo(direction, Vector3.Up);
        return angle;
    }

    public void SetPixelFrog(bool is_pixel)
    {
        CuteFrogCharacter.Hide();
        PixelFrogCharacter.Hide();
        Frog = is_pixel ? PixelFrogCharacter : CuteFrogCharacter;
        Frog.Show();
    }

    public Coroutine AnimateEatTarget(Node3D target)
    {
        return Frog.AnimateEatTarget(target);
    }

    public void SetCoveringEyes(bool is_covering)
    {
        Frog.SetCoveringEyes(is_covering);
    }
}
