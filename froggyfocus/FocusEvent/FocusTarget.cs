using Godot;
using System.Collections;

public partial class FocusTarget : Node3D
{
    public FocusCharacterInfo Info { get; private set; }
    public FocusCharacter Character { get; private set; }
    public float Size { get; private set; }
    public float Radius => Size * 0.5f;

    private Coroutine _cr_moving;
    private FocusEvent _event;
    private RandomNumberGenerator _rng = new();

    public void Initialize(FocusEvent focus_event)
    {
        _event = focus_event;
    }

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

    public void StartMoving()
    {
        _cr_moving = this.StartCoroutine(MovingCr, "moving");
    }

    public void StopMoving()
    {
        Coroutine.Stop(_cr_moving);
    }

    private IEnumerator MovingCr()
    {
        while (true)
        {
            // Select position
            var position = GetRandomPosition(Info.MoveAxis);
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
            var delay = _rng.RandfRange(Info.MoveDelayRange.X, Info.MoveDelayRange.Y);
            yield return new WaitForSeconds(delay);
        }
    }

    private void Move(Vector3 velocity)
    {
        GlobalPosition += velocity;
    }

    private Vector3 GetRandomPosition(FocusEventAxis axis) => axis switch
    {
        FocusEventAxis.XZ => GetRandomPosition(1, 0, 1),
        FocusEventAxis.XY => GetRandomPosition(1, 1, 0),
        FocusEventAxis.X => GetRandomPosition(1, 0, 0),
        _ => Vector3.Zero
    };

    private Vector3 GetRandomPosition(int x, int y, int z) => GetRandomPosition(new Vector3I(x, y, z));
    private Vector3 GetRandomPosition(Vector3I mul)
    {
        var rx = _event.Size.X;
        var ry = _event.Size.Y;
        var x = mul.X * _rng.RandfRange(-rx, rx);
        var y = mul.Y * _rng.RandfRange(-ry, ry);
        var z = mul.Z * _rng.RandfRange(-ry, ry);
        var position = _event.GlobalPosition + _event.Offset + new Vector3(x, y, z);
        return position;
    }
}
