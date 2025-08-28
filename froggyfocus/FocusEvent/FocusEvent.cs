using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class FocusEvent : Node3D
{
    [Export]
    public string Id;

    [Export]
    public FocusEventInfo Info;

    [Export]
    public Camera3D Camera;

    [Export]
    public FocusCursor Cursor;

    [Export]
    public FocusTarget Target;

    [Export]
    public FrogCharacter Frog;

    [Export]
    public FocusSkillCheck OverrideSkillCheck;

    [Export]
    public Node3D SkillCheckParent;

    public FocusCharacterInfo DebugTargetInfo { get; set; }
    public int? DebugTargetStars { get; set; }

    public event Action<FocusEventCompletedResult> OnCompleted;
    public event Action<FocusEventFailedResult> OnFailed;
    public event Action OnStarted;
    public event Action OnEnabled;
    public event Action OnDisabled;

    private List<FocusSkillCheck> skill_checks = new();

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

        InitializeSkillChecks();
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        Player.SetAllLocks(nameof(FocusEvent), false);
    }

    private void InitializeSkillChecks()
    {
        skill_checks = SkillCheckParent.GetNodesInChildren<FocusSkillCheck>();
        skill_checks.ForEach(x => x.Initialize(this));
    }

    private void CreateTarget()
    {
        var info = DebugTargetInfo ?? Info.GetRandomCharacter();
        DebugTargetInfo = null;

        var data = InventoryController.Instance.CreateCharacterData(info);

        data.Stars = DebugTargetStars ?? data.Stars;
        DebugTargetStars = null;

        Target.GlobalPosition = GlobalPosition;
        Target.SetData(data);
        Target.Show();
    }

    private void HijackCamera()
    {
        Camera.Current = true;
    }

    public virtual void StartEvent()
    {
        // Disable player
        Player.SetAllLocks(nameof(FocusEvent), true);
        PauseView.ToggleLock.SetLock(nameof(FocusEvent), true);

        this.StartCoroutine(Cr, "event");
        IEnumerator Cr()
        {
            // Target
            CreateTarget();

            // Clear skill checks
            skill_checks.ForEach(x => x.Clear());

            // Transition start
            FocusIntroView.Instance.LoadTarget(Target);
            yield return FocusIntroView.Instance.AnimateShow();

            // Show event
            Show();

            // Hijack camera
            HijackCamera();

            // Transition end
            yield return WaitForRiff();
            yield return FocusIntroView.Instance.AnimateHide();

            // Initialize cursor
            Cursor.GlobalPosition = Target.GlobalPosition;
            Cursor.Start(Target);

            // Start
            EventStarted = true;
            OnStarted?.Invoke();
            FocusEventController.Instance.FocusEventStarted(this);
            this.StartCoroutine(EventCr, "event");
        }
    }

    protected virtual void EndEvent(bool completed)
    {
        this.StartCoroutine(Cr, "event");
        IEnumerator Cr()
        {
            // Disable cursor
            Cursor.Stop();

            yield return new WaitForSeconds(0.25f);

            // Eat target
            FocusOutroView.Instance.CreateTarget(Target);
            yield return FocusOutroView.Instance.EatBugSequence(completed);

            // Camera target player
            Player.Instance.SetCameraTarget();

            // Hide target
            Target.Hide();

            // Enable player
            Player.SetAllLocks(nameof(FocusEvent), false);
            PauseView.ToggleLock.SetLock(nameof(FocusEvent), false);

            // End
            EventStarted = false;

            if (completed)
            {
                FocusEventController.Instance.FocusEventCompleted(new FocusEventCompletedResult(this));
            }
            else
            {
                FocusEventController.Instance.FocusEventFailed(new FocusEventFailedResult(this));
            }

            // Save
            Data.Game.Save();
        }
    }

    private IEnumerator EventCr()
    {
        var rng = new RandomNumberGenerator();

        while (true)
        {
            // Move target
            yield return Target.WaitForMoveToRandomPosition();

            // Wait
            var duration = Target.GetMoveDelay();
            if (duration > 0)
            {
                Target.StopMoving();
                yield return new WaitForSeconds(duration);
            }

            // Skill check
            var has_override_skill_check = OverrideSkillCheck != null;
            var has_skill_checks = Target.Info.SkillChecks?.Count > 0 || has_override_skill_check;
            var is_skill_check = rng.Randf() < 0.5f;
            if (has_skill_checks && is_skill_check)
            {
                Target.StopMoving();
                yield return WaitForSkillCheck();
            }
        }
    }

    private IEnumerator WaitForRiff()
    {
        if (!MusicController.Instance.IsMusicPlaying())
        {
            MusicController.Instance.MuteLock.AddLock(nameof(FocusIntroView));
            FocusIntroView.Instance.PlayRiff();
            yield return new WaitForSeconds(1.0f);
            MusicController.Instance.MuteLock.RemoveLock(nameof(FocusIntroView));
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator WaitForSkillCheck()
    {
        var type =
            (OverrideSkillCheck != null && !OverrideSkillCheck.IsRunning) ? OverrideSkillCheck.Type
            : Target.Info.SkillChecks.Count == 0 ? FocusSkillCheckType.Dash
            : Target.Info.SkillChecks?.Where(x => !skill_checks.FirstOrDefault(y => y.Type == x)?.IsRunning ?? true).ToList().Random();
        var skill_check = skill_checks.FirstOrDefault(x => x.Type == type);

        if (skill_check == null)
        {
            yield return null;
        }
        else
        {
            yield return skill_check.Start();
        }
    }

    private void FocusFilled()
    {
        OnCompleted?.Invoke(new FocusEventCompletedResult(this));

        Data.Game.TargetsCollected++;
        Data.Game.Save();

        EndEvent(true);
    }

    private void FocusEmpty()
    {
        OnFailed?.Invoke(new FocusEventFailedResult(this));
        EndEvent(false);
    }

    private void FocusTarget()
    {
        Frog.SetHandToPosition(Target.GlobalPosition);
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
