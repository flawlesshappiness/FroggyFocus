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
    private bool EventEnabled { get; set; }

    public override void _Ready()
    {
        base._Ready();
        Cursor.OnFocusFilled += FocusFilled;
        Cursor.OnFocusEmpty += FocusEmpty;
        Cursor.OnFocusTarget += FocusTarget;
        Cursor.Initialize(Target, Axis);

        Target.Initialize(this);
    }

    public void EnableEvent() => SetEventEnabled(true);
    public void DisableEvent() => SetEventEnabled(false);
    public void SetEventEnabled(bool enabled)
    {
        EventEnabled = enabled;

        if (enabled)
        {
            CreateTarget();
            FocusEventController.Instance.EnableEvent(this);
        }
        else
        {
            Target.Hide();
            FocusEventController.Instance.DisableEvent(this);
        }
    }

    public void Interact()
    {
        if (!EventEnabled) return;

        StartEvent();
    }

    private void CreateTarget()
    {
        var info = FocusEventController.Instance.Collection.Resources
            .Where(x => x.Axis == Axis)
            .ToList().Random();
        Target.GlobalPosition = TargetMarker.GlobalPosition;
        Target.SetCharacter(info.Characters.PickRandom());
        Target.Show();
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
            Cursor.GlobalPosition = Target.GlobalPosition;
            Cursor.Start(Target);

            // Start target
            Target.StartMoving();

            // Start
            EventStarted = true;
        }
    }

    protected virtual void EndEvent(bool completed)
    {
        this.StartCoroutine(Cr, nameof(EndEvent));
        IEnumerator Cr()
        {
            // Disable cursor
            Cursor.Stop();

            // Stop target
            Target.StopMoving();

            if (completed)
            {
                // Eat target
                yield return Player.Instance.Character.AnimateEatTarget(Target.Character);
            }

            // Hide target
            Target.Hide();

            // Camera target player
            Player.Instance.SetCameraTarget();

            // Enable player
            Player.SetAllLocks(nameof(FocusEvent), false);

            // End
            EventStarted = false;
            DisableEvent();
        }
    }

    private void FocusFilled()
    {
        Data.Game.TargetsCollected++;
        Data.Game.Save();

        FocusEventController.Instance.FocusEventCompleted(this);

        EndEvent(true);
    }

    private void FocusEmpty()
    {
        FocusEventController.Instance.FocusEventFailed(this);

        EndEvent(false);
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
