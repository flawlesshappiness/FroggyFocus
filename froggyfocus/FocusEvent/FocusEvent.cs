using Godot;
using System.Collections;

public partial class FocusEvent : Area3D, IInteractable
{
    [Export]
    public Marker3D CameraMarker;

    [Export]
    public Marker3D PlayerMarker;

    [Export]
    public Marker3D TargetMarker;

    [Export]
    public FocusCursor Cursor;

    [Export]
    public Node3D MockTarget;

    [Export]
    public FocusTarget Target;

    private bool EventStarted { get; set; }

    public override void _Ready()
    {
        base._Ready();
        Cursor.Disable();
        Cursor.OnFocusFilled += FocusFilled;
        Cursor.OnFocusTarget += FocusTarget;
    }

    public void Interact()
    {
        StartEvent();
    }

    protected virtual void StartEvent()
    {
        var info = FocusEventController.Instance.Collection.Resources.Random();

        // Hijack camera
        CameraController.Instance.Target = this;
        CameraController.Instance.Offset = CameraMarker.Position;
        CameraController.Instance.TargetRotation = CameraMarker.GlobalRotationDegrees;

        // Disable player
        Player.SetAllLocks(nameof(FocusEvent), true);

        // Disable mock target
        MockTarget.Disable();

        // Initialize target
        Target.GlobalPosition = TargetMarker.GlobalPosition;
        Target.SetCharacter(info.Characters.PickRandom());
        Target.Enable();

        this.StartCoroutine(Cr, nameof(StartEvent));
        IEnumerator Cr()
        {
            AnimatePlayerToPosition(PlayerMarker, 1f);
            yield return new WaitForSeconds(1f);

            // Initialize cursor
            Cursor.Initialize(Target);
            Cursor.Position = Vector3.Zero;

            // Start target
            Target.StartMoving(GlobalPosition);

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

            // Debug
            MockTarget.Enable();

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
