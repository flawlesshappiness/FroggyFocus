using Godot;
using System.Collections;

public partial class FocusTarget : Node3D
{
    public FocusCharacterInfo Info { get; private set; }
    public FocusCharacter Character { get; private set; }
    public float Size { get; private set; }
    public float Radius => Size * 0.5f;

    private Vector3 _center;
    private Coroutine _cr_moving;

    public void SetCharacter(FocusCharacterInfo info)
    {
        RemoveCharacter();

        Info = info;
        Character = info.Scene.Instantiate<FocusCharacter>();
        Character.SetParent(this);
        Character.Position = Vector3.Zero;
        Character.Rotation = Vector3.Zero;

        RandomizeSize();
    }

    private void RemoveCharacter()
    {
        if (Character == null) return;

        Character.QueueFree();
        Character = null;
    }

    public void RandomizeSize()
    {
        var rng = new RandomNumberGenerator();
        var size = rng.RandfRange(Info.SizeRange.X, Info.SizeRange.Y);
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
            var dir_to_position_clamped = dir_to_position.ClampMagnitude(Info.MoveLengthRange.X, Info.MoveLengthRange.Y);
            position = GlobalPosition + dir_to_position_clamped;

            // Move to position
            Character.StartFacingDirection(dir_to_position);

            while (GlobalPosition.DistanceTo(position) > 0.1f)
            {
                Move(dir_to_position.Normalized() * Info.MoveSpeed * GameTime.DeltaTime);
                yield return null;
            }

            // Wait
            var delay = rng.RandfRange(Info.MoveDelayRange.X, Info.MoveDelayRange.Y);
            yield return new WaitForSeconds(delay);
        }
    }

    private void Move(Vector3 velocity)
    {
        GlobalPosition += velocity;
    }
}
