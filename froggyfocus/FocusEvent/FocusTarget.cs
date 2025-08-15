using Godot;
using System.Collections;
using System.Linq;

public partial class FocusTarget : Node3D
{
    [Export]
    public GpuParticles3D PsWaterRipples;

    public FocusCharacterInfo Info { get; private set; }
    public FocusCharacter Character { get; private set; }
    public float Size { get; private set; }
    public int Stars { get; private set; }
    public float Difficulty { get; private set; }
    public int Reward { get; private set; }
    public float Radius => Size * 0.5f;

    private float UpdatedMoveSpeed { get; set; }

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
        UpdateMoveSpeed();

        RandomizeSize();
    }

    private void UpdateDifficulty()
    {
        var start = rng.RandiRange(1, 3);
        var hotspot = Player.Instance.HasHotspot ? 1 : 0;
        var stars = Mathf.Clamp(start + hotspot, 1, 5);
        SetStars(stars);
    }

    public void SetStars(int stars)
    {
        var difficulty = Mathf.Clamp((stars - 1) / 4f, 0, 1);
        Stars = stars;
        Difficulty = difficulty;
        Reward = (int)(Info.CurrencyReward + (stars * 5));
    }

    private void UpdateMoveSpeed()
    {
        UpdatedMoveSpeed = Info.MoveSpeedRange.Range(Difficulty);
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
        var position = GetNextPosition();
        var dir_to_position = GlobalPosition.DirectionTo(position);

        // Move to position
        Character.StartFacingDirection(dir_to_position);
        Character.SetMoving(true);

        while (GlobalPosition.DistanceTo(position) > 0.1f)
        {
            Move(dir_to_position.Normalized() * UpdatedMoveSpeed * GameTime.DeltaTime);
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

    private float GetDifficultyRange(Vector2 range)
    {
        var variance = rng.RandfRange(-0.1f, 0f);
        var t = Mathf.Clamp(Difficulty + variance, 0, 1);
        return range.Range(t);
    }

    public float GetMoveLength() => GetDifficultyRange(Info.MoveLengthRange);
    public float GetMoveDelay() => GetDifficultyRange(Info.MoveDelayRange);

    public Vector3 GetApproximatePosition(Vector3 position)
    {
        return NavigationServer3D.MapGetClosestPoint(NavigationServer3D.GetMaps().First(), position).Set(y: position.Y);
    }

    public Vector3 GetRandomPosition()
    {
        var center = focus_event.GlobalPosition;
        var rx = 5;
        var rz = 2f;
        var x = rng.RandfRange(-rx, rx);
        var z = rng.RandfRange(-rz, rz);
        var position = center + new Vector3(x, 0, z);
        return position;
    }

    public Vector3 GetNextDirection()
    {
        var position = GetRandomPosition();
        var dir = GlobalPosition.DirectionTo(position).Normalized();
        return dir;
    }

    public Vector3 GetNextPosition()
    {
        var dir = GetNextDirection();
        var length = GetMoveLength();
        var position = GetApproximatePosition(GlobalPosition + dir * length);
        return position;
    }

    private void UpdateSwimmer()
    {
        var is_swimmer = Info.Tags.Contains(FocusCharacterTag.Water);
        PsWaterRipples.Emitting = is_swimmer;
    }
}
