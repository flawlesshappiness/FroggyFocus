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

    private bool EventStarted { get; set; }
    private FocusTarget Target { get; set; }

    public override void _Ready()
    {
        base._Ready();
        Cursor.Disable();
        Cursor.OnFocusFilled += FocusFilled;
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

        // Create target
        Target = info.Target.Instantiate<FocusTarget>();
        Target.SetParent(this);
        Target.GlobalPosition = TargetMarker.GlobalPosition;

        // Position target


        this.StartCoroutine(Cr, nameof(StartEvent));
        IEnumerator Cr()
        {
            AnimatePlayerToPosition(PlayerMarker, 1f);
            yield return new WaitForSeconds(1f);

            // Enable cursor
            Cursor.Initialize();
            Cursor.Target = Target;

            // Start
            EventStarted = true;
        }
    }

    protected virtual void EndEvent()
    {
        this.StartCoroutine(Cr, nameof(EndEvent));
        IEnumerator Cr()
        {
            yield return Player.Instance.Character.AnimateTongueTowards(Target);

            // Disable target
            Target.QueueFree();

            yield return Player.Instance.Character.AnimageTongueBack();

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
                player.RotateToDirection(-end.DirectionTo(Target.GlobalPosition));
            });
        }
    }
}
