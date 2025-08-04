using Godot;
using System.Collections;

public partial class FocusTarget : Node3D
{
    [Export]
    public GpuParticles3D PsWaterRipples;

    public FocusCharacterInfo Info { get; private set; }
    public FocusCharacter Character { get; private set; }
    public float Size { get; private set; }
    public float Difficulty { get; private set; }
    public int Reward { get; private set; }
    public float Radius => Size * 0.5f;

    private FocusEvent focus_event;
    private RandomNumberGenerator rng = new();

    public void Initialize(FocusEvent focus_event)
    {
        this.focus_event = focus_event;
    }

    public void SetCharacter(FocusCharacterInfo info)
    {
        RemoveCharacter();

        Info = info;
        Character = info.Scene.Instantiate<FocusCharacter>();
        Character.Initialize(info);
        Character.SetParent(this);
        Character.ClearPositionAndRotation();

        UpdateSwimmer();
        UpdateDifficulty();

        RandomizeSize();
    }

    private void UpdateDifficulty()
    {
        var variation = rng.RandfRange(-0.1f, 0.1f);
        var hotspot = Player.Instance.HasHotspot ? 0.2f : 0.0f;
        var extra = variation + hotspot;
        Difficulty = Mathf.Clamp(Info.Difficulty + extra, 0, 1);
        Reward = (int)(Info.CurrencyReward * (1f + extra));
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

    public IEnumerator WaitForMoveToRandomPosition()
    {
        // Select position
        var position = GetClampedPosition();
        var dir_to_position = GlobalPosition.DirectionTo(position);

        // Move to position
        Character.StartFacingDirection(dir_to_position);
        Character.SetMoving(true);

        while (GlobalPosition.DistanceTo(position) > 0.1f)
        {
            Move(dir_to_position.Normalized() * Info.MoveSpeed * GameTime.DeltaTime);
            yield return null;
        }
    }

    public void StopMoving()
    {
        Character.SetMoving(false);
    }

    private void Move(Vector3 velocity)
    {
        GlobalPosition += velocity;
    }

    public Vector3 GetClampedPosition()
    {
        var position = GetRandomPosition();
        var dir_to_position = GlobalPosition.DirectionTo(position);
        var length = dir_to_position.Length();
        var dir_to_position_clamped = dir_to_position.ClampMagnitude(Info.MoveLengthRange.X, Info.MoveLengthRange.Y);
        return GlobalPosition + dir_to_position_clamped;
    }

    public Vector3 GetRandomPosition() => GetRandomPosition(1, 0, 1);
    private Vector3 GetRandomPosition(int x, int y, int z) => GetRandomPosition(new Vector3I(x, y, z));
    private Vector3 GetRandomPosition(Vector3I mul)
    {
        var rx = focus_event.Size.X;
        var ry = focus_event.Size.Y;
        var x = mul.X * rng.RandfRange(-rx, rx);
        var y = mul.Y * rng.RandfRange(-ry, ry);
        var z = mul.Z * rng.RandfRange(-ry, ry);
        var center = focus_event.GlobalPosition;
        var offset = focus_event.Offset;
        var position = center + offset + new Vector3(x, y, z);
        return position;
    }

    private void UpdateSwimmer()
    {
        var is_swimmer = Info.Tags.Contains(FocusCharacterTag.Water);
        PsWaterRipples.Emitting = is_swimmer;
    }
}
