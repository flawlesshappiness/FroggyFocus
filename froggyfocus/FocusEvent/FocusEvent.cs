using Godot;
using System;
using System.Collections;

public partial class FocusEvent : Node3D
{
    [Export]
    public FocusEventInfo Info;

    [Export]
    public Vector2 Size;

    [Export]
    public Vector3 Offset;

    [Export]
    public Camera3D Camera;

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

    public override void _Ready()
    {
        base._Ready();
        Cursor.OnFocusFilled += FocusFilled;
        Cursor.OnFocusEmpty += FocusEmpty;
        Cursor.OnFocusTarget += FocusTarget;
        Cursor.Initialize(Target);

        Target.Initialize(this);
    }

    private void CreateTarget()
    {
        Target.GlobalPosition = TargetMarker.GlobalPosition;
        Target.SetCharacter(Info.Characters.PickRandom());
        Target.Show();
    }

    private void HijackCamera()
    {
        Camera.Current = true;
    }

    public virtual void StartEvent()
    {
        // Target
        CreateTarget();

        // Hijack camera
        HijackCamera();

        // Disable player
        Player.SetAllLocks(nameof(FocusEvent), true);

        this.StartCoroutine(Cr, "event");
        IEnumerator Cr()
        {
            //AnimatePlayerToPosition(PlayerMarker, 1f);

            // Initialize cursor
            Cursor.GlobalPosition = Target.GlobalPosition;
            Cursor.Start(Target);

            // Start target
            //Target.Start();

            // Start
            EventStarted = true;
            OnStarted?.Invoke();
            FocusEventController.Instance.FocusEventStarted(this);
            this.StartCoroutine(EventCr, "event");

            yield return null;
        }
    }

    protected virtual void EndEvent(bool completed)
    {
        this.StartCoroutine(Cr, "event");
        IEnumerator Cr()
        {
            // Disable cursor
            Cursor.Stop();

            // Camera target player
            Player.Instance.SetCameraTarget();

            // Stop target
            //Target.Stop();

            if (completed)
            {
                // Eat target
                //yield return Player.Instance.Character.AnimateEatTarget(Target.Character);
                CurrencyController.Instance.AddValue(CurrencyType.Money, Target.Info.CurrencyReward);
            }

            // Hide target
            Target.Hide();

            // Enable player
            Player.SetAllLocks(nameof(FocusEvent), false);

            // End
            EventStarted = false;
            FocusEventController.Instance.FocusEventCompleted(new FocusEventCompletedResult(this));
            yield return null;
        }
    }

    IEnumerator EventCr()
    {
        var rng = new RandomNumberGenerator();

        while (true)
        {
            // Move target
            yield return Target.WaitForMoveToRandomPosition();

            // Skill check / wait
            var is_skill_check = rng.Randf() < 0.5f;
            if (is_skill_check && false)
            {
                // Do skill check
                yield return null;
            }
            else
            {
                var duration = rng.RandfRange(Target.Info.MoveDelayRange.X, Target.Info.MoveDelayRange.Y);
                yield return new WaitForSeconds(duration);
            }
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
