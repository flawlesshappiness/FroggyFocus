using Godot;

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
    private Node3D Target { get; set; }

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
        CameraController.Instance.Target = null;
        CameraController.Instance.GlobalPosition = CameraMarker.GlobalPosition;
        CameraController.Instance.GlobalRotationDegrees = CameraMarker.GlobalRotationDegrees;

        // Disable player
        Player.SetAllLocks(nameof(FocusEvent), true);

        // Position player
        Player.Instance.GlobalPosition = PlayerMarker.GlobalPosition;

        // Disable mock target
        MockTarget.Disable();

        // Create target
        Target = info.Target.Instantiate<Node3D>();
        Target.SetParent(this);
        Target.GlobalPosition = TargetMarker.GlobalPosition;

        // Position target

        // Enable cursor
        Cursor.Initialize();
        Cursor.Target = Target;

        EventStarted = true;
    }

    protected virtual void EndEvent()
    {
        // Camera target player
        Player.Instance.SetCameraTarget();

        // Enable player
        Player.SetAllLocks(nameof(FocusEvent), false);

        // Disable cursor
        Cursor.Disable();
        Cursor.InputEnabled = false;

        // Disable target
        Target.QueueFree();
        MockTarget.Enable();

        EventStarted = false;
    }

    private void FocusFilled()
    {
        EndEvent();
    }
}
