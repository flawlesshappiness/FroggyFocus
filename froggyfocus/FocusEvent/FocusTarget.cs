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
    private Curve3D move_curve = new();

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

        CalculateMovePath(position);

        // Move to position
        Character.SetMoving(true);

        var is_glitch = Info.IsGlitch;
        var velocity = Vector3.Zero;
        var start = GlobalPosition;
        var length = move_curve.GetBakedLength();
        var dist = 0f;
        while (dist < length)
        {
            var speed_mul = is_glitch ? 1.0f : GameTime.DeltaTime;
            var sample = move_curve.SampleBaked(dist);
            velocity = (start + sample) - GlobalPosition;
            Move(velocity);
            dist += UpdatedMoveSpeed * speed_mul;

            if (is_glitch)
            {
                Character.RotateToDirectionImmediate(velocity);
                yield return new WaitForSeconds(0.3f);
            }
            else
            {
                Character.StartFacingDirection(velocity);
                yield return null;
            }
        }

        velocity = (start + move_curve.SampleBaked(length)) - GlobalPosition;
        Move(velocity);
        yield return null;
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

    private void CalculateMovePath(Vector3 global_destination)
    {
        if (Info.MoveType == FocusCharacterMoveType.Flying)
        {
            CalculateCurvedMovePath(global_destination);
        }
        else
        {
            CalculateStraightMovePath(global_destination);
        }
    }

    private void CalculateCurvedMovePath(Vector3 global_destination)
    {
        var destination = global_destination - GlobalPosition;
        var dir = destination.Normalized();
        var perp = dir.Cross(Vector3.Up).Normalized();
        var dir_face = -Character.GlobalBasis.Z;

        var width_mul = Mathf.Lerp(0.25f, 0.4f, Difficulty);
        var width = dir.Length() * width_mul;

        var p1 = Vector3.Zero;
        var p3 = dir;
        var p2 = p1.Lerp(p3, 0.5f);

        var p1_out = dir_face * 0.5f;
        var p2_in = ((p1 - p2) * 0.5f + perp * width);
        var p2_out = (p2.Lerp(p3, 0.5f) - perp * width) - p2;

        move_curve.ClearPoints();
        move_curve.AddPoint(p1, @out: p1_out);
        move_curve.AddPoint(p2, @in: p2_in, @out: p2_out);
        move_curve.AddPoint(p3);
    }

    private void CalculateStraightMovePath(Vector3 global_destination)
    {
        var destination = global_destination - GlobalPosition;
        var dir = destination.Normalized();
        var dir_face = -Character.GlobalBasis.Z;

        var p1 = Vector3.Zero;
        var p2 = dir;

        var p1_out = dir_face * 0.5f;

        move_curve.ClearPoints();
        move_curve.AddPoint(p1, @out: p1_out);
        move_curve.AddPoint(p2);
    }
}
