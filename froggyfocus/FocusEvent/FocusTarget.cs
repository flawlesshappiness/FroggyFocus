using Godot;
using System.Collections;

public partial class FocusTarget : Node3D
{
    [Export]
    public Vector2 SizeRange = new Vector2(1, 1);

    public FocusCharacter Character { get; set; }
    public float Size { get; set; }
    public float Radius => Size * 0.5f;

    private Vector3 _center;
    private Coroutine _cr_moving;

    public override void _Ready()
    {
        base._Ready();
        SetRandomSize();
    }

    public void SetCharacter(FocusCharacterInfo info)
    {
        RemoveCharacter();

        Character = info.Scene.Instantiate<FocusCharacter>();
        Character.SetParent(this);
        Character.Position = Vector3.Zero;
        Character.Rotation = Vector3.Zero;
    }

    private void RemoveCharacter()
    {
        if (Character == null) return;

        Character.QueueFree();
        Character = null;
    }

    public void SetRandomSize()
    {
        var rng = new RandomNumberGenerator();
        var size = rng.RandfRange(SizeRange.X, SizeRange.Y);
        SetSize(size);
    }

    public void SetSize(float size)
    {
        Size = size;
        Scale = Vector3.One * size;
    }

    public void StartMoving(Vector3 center)
    {
        _center = center;
        _cr_moving = this.StartCoroutine(MovingCr, "moving");
    }

    public void StopMoving()
    {
        Coroutine.Stop(_cr_moving);
    }

    protected virtual IEnumerator MovingCr()
    {
        var move_speed = 1f;
        var delay_duration = 1f;
        var rng = new RandomNumberGenerator();

        while (true)
        {
            // Select position
            var radius = 5f;
            var x = rng.RandfRange(-radius, radius);
            var z = rng.RandfRange(-radius, radius);
            var position = _center + new Vector3(x, 0, z);
            var dir_to_position = GlobalPosition.DirectionTo(position);
            var length = dir_to_position.Length();
            var dir_to_position_clamped = dir_to_position.ClampMagnitude(1f, 2f);
            position = GlobalPosition + dir_to_position_clamped;

            // Move to position
            Character.StartFacingDirection(dir_to_position);

            while (GlobalPosition.DistanceTo(position) > 0.1f)
            {
                Move(dir_to_position.Normalized() * move_speed * GameTime.DeltaTime);
                yield return null;
            }

            // Wait
            yield return new WaitForSeconds(delay_duration);
        }
    }

    private void Move(Vector3 velocity)
    {
        GlobalPosition += velocity;
    }
}
