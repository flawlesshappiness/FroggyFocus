using Godot;
using System.Collections;
using System.Linq;

public partial class FocusEvent : Area3D, IInteractable
{
    [Export]
    public FocusEventAxis Axis;

    [Export]
    public Vector2 Size;

    [Export]
    public Vector3 Offset;

    [Export]
    public Marker3D CameraMarker;

    [Export]
    public Marker3D PlayerMarker;

    [Export]
    public Marker3D TargetMarker;

    [Export]
    public FocusCursor Cursor;

    [Export]
    public FocusTarget Target;

    private bool EventStarted { get; set; }

    public override void _Ready()
    {
        base._Ready();
        Cursor.OnFocusFilled += FocusFilled;
        Cursor.OnFocusTarget += FocusTarget;
        Cursor.Initialize(Target, Axis);

        Target.Initialize(this);
        CreateTarget();
    }

    public void Interact()
    {
        StartEvent();
    }

    private void CreateTarget()
    {
        var info = FocusEventController.Instance.Collection.Resources
            .Where(x => x.Axis == Axis)
            .ToList().Random();
        Target.GlobalPosition = TargetMarker.GlobalPosition;
        Target.SetCharacter(info.Characters.PickRandom());
        Target.Enable();
    }

    protected virtual void StartEvent()
    {
        // Hijack camera
        CameraController.Instance.Target = this;
        CameraController.Instance.Offset = CameraMarker.Position;
        CameraController.Instance.TargetRotation = CameraMarker.GlobalRotationDegrees;

        // Disable player
        Player.SetAllLocks(nameof(FocusEvent), true);

        this.StartCoroutine(Cr, nameof(StartEvent));
        IEnumerator Cr()
        {
            AnimatePlayerToPosition(PlayerMarker, 1f);
            yield return new WaitForSeconds(1f);

            // Initialize cursor
            Cursor.Position = Offset;
            Cursor.Start(Target);

            // Start target
            Target.StartMoving();

            // Start
            EventStarted = true;
        }
    }

    protected virtual void EndEvent()
    {
        this.StartCoroutine(Cr, nameof(EndEvent));
        IEnumerator Cr()
        {
            // Stop target
            Target.StopMoving();

            // Eat target
            yield return Player.Instance.Character.AnimateEatTarget(Target.Character);

            // Remove target
            Target.Character.Disable();

            // Camera target player
            Player.Instance.SetCameraTarget();

            // Enable player
            Player.SetAllLocks(nameof(FocusEvent), false);

            // Disable cursor
            Cursor.Disable();
            Cursor.InputEnabled = false;

            // New target
            CreateTarget();

            // End
            EventStarted = false;
        }
    }

    private void FocusFilled()
    {
        EndEvent();
    }

    private void FocusTarget()
    {
        Player.Instance.Character.StartFacingPosition(Target.GlobalPosition);
    }

    private Coroutine AnimatePlayerToPosition(Node3D node, float duration)
    {
        return this.StartCoroutine(Cr, nameof(AnimatePlayerToPosition));
        IEnumerator Cr()
        {
            var player = Player.Instance;
            var start = player.GlobalPosition;
            var end = node.GlobalPosition;
            yield return LerpEnumerator.Lerp01(duration, f =>
            {
                player.GlobalPosition = start.Lerp(end, f);
                player.Character.StartFacingDirection(end.DirectionTo(Target.GlobalPosition));
            });
        }
    }
}
