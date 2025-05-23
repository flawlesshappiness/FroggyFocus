using Godot;
using System;
using System.Collections;

public partial class FocusEvent : Area3D, IInteractable
{
    [Export]
    public FocusEventInfo Info;

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

    public event Action<FocusEventCompletedResult> OnCompleted;
    public event Action<FocusEventFailedResult> OnFailed;
    public event Action OnStarted;
    public event Action OnEnabled;
    public event Action OnDisabled;

    private bool EventStarted { get; set; }
    private bool EventEnabled { get; set; }

    private Coroutine cr_active;

    public override void _Ready()
    {
        base._Ready();
        Cursor.OnFocusFilled += FocusFilled;
        Cursor.OnFocusEmpty += FocusEmpty;
        Cursor.OnFocusTarget += FocusTarget;
        Cursor.Initialize(Target);

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
            DisableEventAfterDelay();
            OnEnabled?.Invoke();
        }
        else
        {
            Target.Hide();
            OnDisabled?.Invoke();
        }
    }

    public void EnableEventAfterDelay()
    {
        cr_active = this.StartCoroutine(Cr, "active");
        IEnumerator Cr()
        {
            var rng = new RandomNumberGenerator();
            var delay = rng.RandfRange(10, 20);
            yield return new WaitForSeconds(delay);
            EnableEvent();
        }
    }

    public void DisableEventAfterDelay()
    {
        cr_active = this.StartCoroutine(Cr, "active");
        IEnumerator Cr()
        {
            var rng = new RandomNumberGenerator();
            var delay = rng.RandfRange(60, 90);
            yield return new WaitForSeconds(delay);
            DisableEvent();
        }
    }

    private void StopActiveCoroutine()
    {
        Coroutine.Stop(cr_active);
    }

    public void Interact()
    {
        if (!EventEnabled) return;

        StartEvent();
    }

    private void CreateTarget()
    {
        Target.GlobalPosition = TargetMarker.GlobalPosition;
        Target.SetCharacter(Info.Characters.PickRandom());
        Target.Show();
        Target.StartMoving(0.2f);
    }

    protected virtual void StartEvent()
    {
        StopActiveCoroutine();

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
            OnStarted?.Invoke();
        }
    }

    protected virtual void EndEvent(bool completed)
    {
        this.StartCoroutine(Cr, nameof(EndEvent));
        IEnumerator Cr()
        {
            // Disable cursor
            Cursor.Stop();

            // Camera target player
            Player.Instance.SetCameraTarget();

            // Stop target
            Target.StopMoving();

            if (completed)
            {
                // Eat target
                yield return Player.Instance.Character.AnimateEatTarget(Target.Character);
            }

            // Hide target
            Target.Hide();

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

        OnCompleted?.Invoke(new FocusEventCompletedResult(this));
        EndEvent(true);
    }

    private void FocusEmpty()
    {
        OnFailed?.Invoke(new FocusEventFailedResult(this));
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
