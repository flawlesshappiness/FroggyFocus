using Godot;
using System.Collections;
using System.Linq;

public partial class FocusTarget : Node3D
{
    [Export]
    public GpuParticles3D PsWaterRipples;

    public FocusCharacterInfo Info { get; private set; }
    public FocusCharacter Character { get; private set; }
    public InventoryCharacterData CharacterData { get; private set; }
    public float Difficulty { get; private set; }
    public float Radius => CharacterData.Size * 0.5f;

    public float UpdatedMoveSpeed { get; private set; }

    private FocusEvent focus_event;
    private RandomNumberGenerator rng = new();

    public void Initialize(FocusEvent focus_event)
    {
        this.focus_event = focus_event;
    }

    public void SetData(InventoryCharacterData data)
    {
        CharacterData = data;
        Info = FocusCharacterController.Instance.GetInfoFromPath(data.InfoPath);
        SetCharacter(Info);
        UpdateSwimmer();
        UpdateDifficulty();
        UpdateMoveSpeed();
    }

    private void SetCharacter(FocusCharacterInfo info)
    {
        RemoveCharacter();

        Info = info;
        Character = info.Scene.Instantiate<FocusCharacter>();
        Character.Initialize(info);
        Character.SetParent(this);
        Character.ClearPositionAndRotation();

        Scale = Vector3.One * CharacterData.Size;
    }

    private void UpdateDifficulty()
    {
        Difficulty = Mathf.Clamp((CharacterData.Stars - 1) / 4f, 0, 1);
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
        var rx = 4;
        var rz = 2;
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
