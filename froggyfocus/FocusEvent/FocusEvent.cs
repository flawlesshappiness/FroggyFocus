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

    [Export]
    public AnimationPlayer AnimationPlayer_Camera;

    [Export]
    public AnimationPlayer AnimationPlayer_Frog;

    [Export]
    public AudioStreamPlayer SfxSuspenseNormal_Start;

    [Export]
    public AudioStreamPlayer SfxSuspenseFast_Start;

    [Export]
    public AudioStreamPlayer SfxSuspenseNormal_Success;

    [Export]
    public AudioStreamPlayer SfxSuspenseNormal_Fail;

    public FocusCharacterInfo OverrideTargetInfo { get; set; }
    public int? OverrideTargetStars { get; set; }

    public event Action<FocusEventCompletedResult> OnCompleted;
    public event Action<FocusEventFailedResult> OnFailed;
    public event Action OnStopped;
    public event Action OnStarted;
    public event Action OnEnabled;
    public event Action OnDisabled;

    private List<FocusSkillCheck> skill_checks = new();

    private bool IsFastCutscene => Data.Options.CutsceneTypeIndex == 1;
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

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (PlayerInput.Pause.Released)
        {
            EndEventPrematurely();
        }
    }

    private void CreateTarget()
    {
        var info = OverrideTargetInfo ?? Info.GetRandomCharacter();
        OverrideTargetInfo = null;

        var data = InventoryController.Instance.CreateCharacterData(info);

        data.Stars = OverrideTargetStars ?? data.Stars;
        OverrideTargetStars = null;

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

        this.StartCoroutine(Cr, "event");
        IEnumerator Cr()
        {
            // Target
            CreateTarget();

            // Clear skill checks
            skill_checks.ForEach(x =>
            {
                x.Clear();
                x.ResetCooldown();
            });

            // Transition start
            FocusIntroView.Instance.LoadTarget(Target);
            yield return FocusIntroView.Instance.AnimateShow();

            // Show event
            Show();

            // Hijack camera
            HijackCamera();

            // Game View
            GameView.Instance.SetFocusEventControlsVisible(true);

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
            this.StartCoroutine(EventCr, "event")
                .SetRunWhilePaused(true);
        }
    }

    protected virtual void EndEvent(bool completed)
    {
        OnStopped?.Invoke();

        this.StartCoroutine(Cr, "event");
        IEnumerator Cr()
        {
            // Disable cursor
            Cursor.Stop();

            if (completed)
            {
                yield return WaitForCatchTarget();
            }
            else
            {
                yield return WaitForLoseTarget();
            }

            // Transition
            yield return WaitForTransition();

            // Camera target player
            Player.Instance.SetCameraTarget();

            // Hide target
            Target.Hide();

            // GameView
            GameView.Instance.SetFocusEventControlsVisible(false);

            // Enable player
            Player.SetAllLocks(nameof(FocusEvent), false);

            // End
            EventStarted = false;

            if (completed)
            {
                var result = new FocusEventCompletedResult(this);
                OnCompleted?.Invoke(result);
                FocusEventController.Instance.FocusEventCompleted(result);
            }
            else
            {
                var result = new FocusEventFailedResult(this);
                OnFailed?.Invoke(result);
                FocusEventController.Instance.FocusEventFailed(result);
            }

            // Save
            Data.Game.Save();
        }
    }

    private void EndEventPrematurely()
    {
        if (!EventStarted) return;

        Cursor.AdjustFocusValue(-99999);
    }

    private IEnumerator EventCr()
    {
        var rng = new RandomNumberGenerator();

        while (true)
        {
            // Move target
            if (!Target.Info.IsStationary)
            {
                yield return Target.WaitForMoveToRandomPosition();
            }

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
            var is_skill_check = true || rng.Randf() < 0.5f;
            if (has_skill_checks && is_skill_check)
            {
                Target.StopMoving();
                yield return WaitForSkillCheck();
            }
        }
    }

    private IEnumerator WaitForRiff()
    {
        if (Data.Options.CutsceneTypeIndex == 1) // Fast
        {
            yield return new WaitForSeconds(0.25f);
        }
        else if (!MusicController.Instance.IsMusicPlaying())
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
            (OverrideSkillCheck != null && OverrideSkillCheck.IsAvailable()) ? OverrideSkillCheck.Type
            : Target.Info.SkillChecks.Count == 0 ? FocusSkillCheckType.Dash
            : Target.Info.SkillChecks?.Where(x => skill_checks.FirstOrDefault(y => y.Type == x)?.IsAvailable() ?? true).ToList().Random();
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
        Data.Game.TargetsCollected++;
        Data.Game.Save();

        EndEvent(true);
    }

    private void FocusEmpty()
    {
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

    private IEnumerator WaitForCatchTarget()
    {
        Frog.SetHandsBack();
        Target.Animate_Scared();
        yield return WaitForSuspense();
        Target.Animate_Unscared();
        SfxSuspenseNormal_Success.Play();
        GameView.Instance.AnimateVignetteHide(0.25f);
        AnimateZoomOut(0.25f);
        yield return Frog.AnimateEatTarget(Target.Character);
        yield return FocusOutroView.Instance.WaitForInventory(Target);
        yield return new WaitForSeconds(0.25f);
    }

    private IEnumerator WaitForLoseTarget()
    {
        Frog.SetHandsBack();
        yield return WaitForSuspense();
        SfxSuspenseNormal_Fail.Play();
        GameView.Instance.AnimateVignetteHide(0.25f);
        AnimateZoomOut(0.25f);
        AnimationPlayer_Frog.Play("scared");
        Frog.SetMouthOpen(true);
        yield return Target.Animate_Disappear();
        yield return new WaitForSeconds(0.25f);
        Frog.SetMouthOpen(false);
    }

    private IEnumerator WaitForSuspense()
    {
        if (IsFastCutscene)
        {
            SfxSuspenseFast_Start.Play();
            GameView.Instance.AnimateVignetteShow(1f);
            yield return AnimateZoomIn(1f);
        }
        else
        {
            SfxSuspenseNormal_Start.Play();
            GameView.Instance.AnimateVignetteShow(2f);
            yield return AnimateZoomIn(2f);
        }
    }

    private IEnumerator WaitForTransition()
    {
        var transition_finished = false;
        TransitionView.Instance.StartTransition(new TransitionSettings
        {
            Type = TransitionType.Color,
            Color = Colors.Black,
            Duration = 0.25f,
            OnTransition = () => { transition_finished = true; }
        });

        while (!transition_finished)
        {
            yield return null;
        }
    }

    private Coroutine AnimateZoomIn(float duration)
    {
        AnimationPlayer_Camera.SpeedScale = 1f / duration;
        return AnimationPlayer_Camera.PlayAndWaitForAnimation("zoom_in");
    }

    private Coroutine AnimateZoomOut(float duration)
    {
        AnimationPlayer_Camera.SpeedScale = 1f / duration;
        return AnimationPlayer_Camera.PlayAndWaitForAnimation("zoom_out");
    }
}
