using Godot;

public partial class FallingPlayer : Node3D
{
    [Export]
    public CuteFrogCharacter Frog;

    private Vector3 Velocity { get; set; }

    public override void _Ready()
    {
        base._Ready();
        Frog.SetFalling();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        Process_Move();
    }

    private void Process_Move()
    {
        var input = PlayerInput.GetMoveInput();
        var input_dir = input.Normalized();
        var dir = new Vector3(input_dir.X, 0, input_dir.Y);
        var acc = 0.1f;
        var v = dir * acc * GameTime.DeltaTime;
        var radius = 1.8f;

        Velocity = (Velocity + v).ClampMagnitude(0f, 1f);
        var position = GlobalPosition + Velocity;

        if (position.Length() > radius)
        {
            var c = Vector3.Zero; // Center
            var p = position.Normalized() * radius; // Collision point
            var n = p.DirectionTo(c); // Normal from collision point
            var cross = n.Cross(Vector3.Up).Normalized(); // Cross between normal and up-vector
            var dot = Velocity.Normalized().Dot(cross); // Dot between velocity and cross
            Velocity = cross * Velocity.Length() * dot * 0.99f; // Update velocity with cross, multiplied by dot and friction
            position = GlobalPosition + Velocity;
        }

        GlobalPosition = position.ClampMagnitude(0f, radius);
    }
}
