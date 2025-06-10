using Godot;
using System.Collections;

public partial class FocusTarget : Node3D
{
    [Export]
    public ExclamationMark ExclamationMark;

    [Export]
    public Node3D StunnedStars;

    [Export]
    public AudioStreamPlayer3D SfxStun;

    public FocusCharacterInfo Info { get; private set; }
    public FocusCharacter Character { get; private set; }
    public float Size { get; private set; }
    public float Radius => Size * 0.5f;

    private float move_mult = 1f;
    private float move_mult_inv = 1f;
    private Coroutine cr_moving;
    private Coroutine cr_action;
    private Coroutine cr_stun;
    private FocusEvent focus_event;
    private RandomNumberGenerator rng = new();

    public void Initialize(FocusEvent focus_event)
    {
        this.focus_event = focus_event;
        StunnedStars.Hide();
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

    public void Start()
    {
        StartMoving();
        StartAction();
    }

    public void Stop()
    {
        StopMoving();
        StopAction();
        StopStun();
    }

    public void StartMoving(float mult = 1f)
    {
        move_mult = mult;
        move_mult_inv = 1f + (1f - mult);
        cr_moving = this.StartCoroutine(MovingCr_Old, "moving");
    }

    public void StopMoving()
    {
        Coroutine.Stop(cr_moving);
        Character.SetMoving(false);
    }

    private IEnumerator MovingCr_Old()
    {
        while (true)
        {
            // Select position
            var position = GetRandomPosition(1, 0, 1);
            var dir_to_position = GlobalPosition.DirectionTo(position);
            var length = dir_to_position.Length();
            var dir_to_position_clamped = dir_to_position.ClampMagnitude(Info.MoveLengthRange.X, Info.MoveLengthRange.Y) * move_mult;
            position = GlobalPosition + dir_to_position_clamped;

            // Move to position
            Character.StartFacingDirection(dir_to_position);
            Character.SetMoving(true);

            while (GlobalPosition.DistanceTo(position) > 0.1f)
            {
                Move(dir_to_position.Normalized() * Info.MoveSpeed * move_mult * GameTime.DeltaTime);
                yield return null;
            }

            Character.SetMoving(false);

            // Wait
            var delay = rng.RandfRange(Info.MoveDelayRange.X, Info.MoveDelayRange.Y) * move_mult_inv;
            yield return new WaitForSeconds(delay);
        }
    }

    public IEnumerator WaitForMoveToRandomPosition()
    {
        // Select position
        var position = GetRandomPosition(1, 0, 1);
        var dir_to_position = GlobalPosition.DirectionTo(position);
        var length = dir_to_position.Length();
        var dir_to_position_clamped = dir_to_position.ClampMagnitude(Info.MoveLengthRange.X, Info.MoveLengthRange.Y);
        position = GlobalPosition + dir_to_position_clamped;

        // Move to position
        Character.StartFacingDirection(dir_to_position);
        Character.SetMoving(true);

        while (GlobalPosition.DistanceTo(position) > 0.1f)
        {
            Move(dir_to_position.Normalized() * Info.MoveSpeed * GameTime.DeltaTime);
            yield return null;
        }

        Character.SetMoving(false);
    }

    public void StartAction()
    {
        cr_action = this.StartCoroutine(ActionCr, "action");
    }

    public void StopAction()
    {
        Coroutine.Stop(cr_action);
    }

    private IEnumerator ActionCr()
    {
        while (true)
        {
            yield return new WaitForSeconds(rng.RandfRange(3f, 10f));

            var action = Info.Actions?.PickRandom();
            if (action != null)
            {
                yield return FocusTargetAction.WaitForAction(action ?? FocusTargetActionType.Stun, this);
            }
        }
    }

    private void Move(Vector3 velocity)
    {
        GlobalPosition += velocity;
    }

    private Vector3 GetRandomPosition(int x, int y, int z) => GetRandomPosition(new Vector3I(x, y, z));
    private Vector3 GetRandomPosition(Vector3I mul)
    {
        var rx = focus_event.Size.X;
        var ry = focus_event.Size.Y;
        var x = mul.X * rng.RandfRange(-rx, rx);
        var y = mul.Y * rng.RandfRange(-ry, ry);
        var z = mul.Z * rng.RandfRange(-ry, ry);
        var center = focus_event.TargetMarker.GlobalPosition;
        var offset = focus_event.Offset * move_mult;
        var position = center + offset + new Vector3(x, y, z);
        return position;
    }

    public void Stun(float duration)
    {
        SfxStun.Play();

        cr_stun = this.StartCoroutine(Cr, "stun");
        IEnumerator Cr()
        {
            StopMoving();
            StunnedStars.Show();
            yield return new WaitForSeconds(duration);
            StunnedStars.Hide();
            StartMoving();
        }
    }

    public void StopStun()
    {
        Coroutine.Stop(cr_stun);
    }
}
