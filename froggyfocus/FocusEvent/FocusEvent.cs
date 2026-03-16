using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FlawLizArt.FocusEvent;

public partial class FocusEvent : Node3D
{
    [Export]
    public Vector2 Size;

    [Export]
    public Camera3D Camera;

    [Export]
    public Camera3D IntroCamera;

    [Export]
    public FocusCursor Cursor;

    [Export]
    public Marker3D CursorStart;

    [Export]
    public Marker3D CameraIntroMarker;

    [Export]
    public PackedScene FocusTargetPrefab;

    [Export]
    public FrogCharacter Frog;

    [Export]
    public FocusSkillCheck OverrideSkillCheck;

    [Export]
    public Node3D SkillCheckParent;

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

    public event Action<FocusEventResult> OnEnded;
    public event Action OnEnabled;
    public event Action OnDisabled;

    public enum State { Idle, Starting, Running, Ending }
    public State CurrentState { get; private set; } = State.Idle;
    public bool IsIdle => CurrentState == State.Idle;
    public bool IsStarting => CurrentState == State.Starting;
    public bool IsRunning => CurrentState == State.Running;
    public bool IsEnding => CurrentState == State.Ending;

    public float TimerDuration { get; private set; }
    public float TimerStart { get; private set; }
    public float TimerEnd { get; private set; }
    public Settings CurrentSettings { get; private set; }
    public List<FocusTarget> Targets { get; private set; } = new();
    public bool IsCoveringEyes { get; private set; }

    private List<FocusSkillCheck> skill_checks = new();
    private List<FocusEventBackground> backgrounds = new();
    private RandomNumberGenerator rng = new();
    private FocusEventResult result;

    private bool IsFastCutscene => Data.Options.CutsceneTypeIndex == 1;

    public class Settings
    {
        public string Id { get; set; }
        public FocusEventInfo EventInfo => event_info ?? (event_info = GetEventInfo());
        public FocusCharacterInfo OverrideTargetInfo { get; set; }
        public int? OverrideTargetStars { get; set; }

        private FocusEventInfo event_info;

        private FocusEventInfo GetEventInfo()
        {
            var info = FocusEventController.Instance.GetInfo(Id);
            if (info == null)
            {
                Debug.LogError($"FocusEvent.Settings: No FocusEventInfo found with id {Id}");
            }

            return info;
        }

        public FocusCharacterInfo GetRandomTargetInfo()
        {
            return OverrideTargetInfo ?? EventInfo.Characters.PickRandom();
        }
    }

    public override void _Ready()
    {
        base._Ready();
        InitializeCursor();
        InitializeSkillChecks();
        InitializeBackgrounds();
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        Player.SetAllLocks(nameof(FocusEvent), false);
    }

    private void InitializeCursor()
    {
        Cursor.Initialize(this);
        Cursor.OnTarget += Cursor_Target;
        Cursor.OnTargetReleased += Cursor_TargetReleased;
    }

    private void InitializeSkillChecks()
    {
        skill_checks = SkillCheckParent.GetNodesInChildren<FocusSkillCheck>();
        skill_checks.ForEach(x => x.Initialize(this));
    }

    private void InitializeBackgrounds()
    {
        backgrounds = this.GetNodesInChildren<FocusEventBackground>();
        HideBackgrounds();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        Process_Timer();
        Process_Facing();
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (!IsRunning) return;

        if (PlayerInput.Pause.Released)
        {
            EndEvent();
        }

        if (PlayerInput.Focus.Pressed)
        {
            SetCoveringEyes(true);
        }
        else if (PlayerInput.Focus.Released)
        {
            SetCoveringEyes(false);
        }
    }

    private void Process_Timer()
    {
        if (!IsRunning) return;
        if (GameTime.Time < TimerEnd) return;

        EndEvent();
    }

    private void Process_Facing()
    {
        if (!IsRunning) return;
        if (!Cursor.HasTarget) return;

        Frog.StartFacingPosition(Cursor.CurrentTarget.GlobalPosition);
    }

    private FocusTarget CreateTarget()
    {
        var info = CurrentSettings.GetRandomTargetInfo();
        var data = InventoryController.Instance.CreateCharacterData(info);
        data.Stars = CurrentSettings.OverrideTargetStars ?? data.Stars;

        var target = FocusTargetPrefab.Instantiate<FocusTarget>();
        target.SetParent(this);
        target.SetData(data);
        target.Show();
        target.Initialize(this);
        target.GlobalPosition = target.GetApproximatePosition(target.GetRandomPosition());

        target.OnCaught += () => Target_Caught(target);

        return target;
    }

    private void CreateTargets()
    {
        ClearTargets();

        var count = CurrentSettings.EventInfo.TargetCount.Range(rng.Randf());
        for (int i = 0; i < count; i++)
        {
            var target = CreateTarget();
            Targets.Add(target);
        }
    }

    private void ClearTargets()
    {
        foreach (var target in Targets)
        {
            target.QueueFree();
        }
        Targets.Clear();
    }

    private void StartTargets()
    {
        Targets.ForEach(x => x.StartState());
    }

    private void StopTargets()
    {
        Targets.ForEach(x => x.StopState());
    }

    private Coroutine HideTargets()
    {
        return this.StartCoroutine(Cr, "hide_targets");
        IEnumerator Cr()
        {
            foreach (var target in Targets)
            {
                if (target.IsCaught) continue;
                if (target.IsFocusMax) continue;

                if (target.Info.Tags.Contains(FocusCharacterTag.Water))
                {
                    target.Animate_DiveDown();
                }
                else if (target.Info.Tags.Contains(FocusCharacterTag.Flying))
                {
                    target.Animate_Disappear();
                }
                else
                {
                    target.Animate_DigDown();
                }

                yield return new WaitForSeconds(0.05f);
            }
        }
    }

    private void Target_Caught(FocusTarget target)
    {
        InventoryController.Instance.AddCharacter(target.CharacterData);
        Data.Game.Save();

        ValidateAllCaught();
    }

    private void ValidateAllCaught()
    {
        var all_caught = Targets.All(x => x.IsCaught);
        if (all_caught)
        {
            EndEvent();
        }
    }

    private void ResetSkillchecks()
    {
        skill_checks.ForEach(x =>
        {
            x.Clear();
            x.ResetCooldown();
        });
    }

    private void HijackCamera()
    {
        Camera.Current = true;
    }

    private void StartCursor()
    {
        Cursor.Load();
        Cursor.Show();
        Cursor.GlobalPosition = CursorStart.GlobalPosition;
    }

    private void StopCursor()
    {
        Cursor.Stop();
    }

    private void StartTimer()
    {
        TimerDuration = 20f; // TODO: Based on upgrade
        TimerStart = GameTime.Time;
        TimerEnd = TimerStart + TimerDuration;
    }

    public void StartEvent(Settings settings)
    {
        CurrentSettings = settings;
        result = new FocusEventResult(this);
        Player.SetAllLocks(nameof(FocusEvent), true);
        CreateTargets();
        TransitionToStart();
        SetBackground(settings.Id);
    }

    private Coroutine TransitionToStart()
    {
        return this.StartCoroutine(Cr, "transition");
        IEnumerator Cr()
        {
            CurrentState = State.Starting;
            yield return AnimateCameraUp();
            Show();
            StartCursor();
            HijackCamera();
            StartTargets();
            StartTimer();
            CurrentState = State.Running;
            FocusEventView.Instance.SetFocusEvent(this);
            FocusEventView.Instance.Show();
            FocusEventController.Instance.FocusEventStarted(this);
        }
    }

    private Coroutine AnimateCameraUp()
    {
        return this.StartCoroutine(Cr, nameof(AnimateCameraUp));
        IEnumerator Cr()
        {
            TransitionView.Instance.StartTransition(new TransitionSettings
            {
                Type = TransitionType.Lilypads,
                Duration = 1f,
            });

            CameraIntroMarker.GlobalPosition = Player.Instance.GlobalPosition.Add(y: 6f);
            var start = Player.Instance.Camera.GlobalTransform;
            var end = CameraIntroMarker.GlobalTransform;
            var curve = Curves.EaseInOutQuad;
            IntroCamera.GlobalTransform = start;
            IntroCamera.Current = true;
            yield return LerpEnumerator.Lerp01(1f, f =>
            {
                var t = curve.Evaluate(f);
                IntroCamera.GlobalTransform = start.InterpolateWith(end, t);
            });
        }
    }

    private void EndEvent()
    {
        if (!IsRunning) return;
        CurrentState = State.Ending;

        StopTargets();
        StopCursor();
        HideTargets();

        TransitionView.Instance.StartTransition(new TransitionSettings
        {
            Type = TransitionType.Lilypads,
            Duration = 1.0f,
            OnTransition = OnTransition
        });

        void OnTransition()
        {
            CurrentState = State.Idle;

            // View
            FocusEventView.Instance.Hide();

            // Player
            Player.Instance.SetCameraTarget();
            Player.SetAllLocks(nameof(FocusEvent), false);

            // Events
            OnEnded?.Invoke(result);
            FocusEventController.Instance.FocusEventEnded(result);

            // Save
            Data.Game.Save();
        }
    }

    private void Cursor_Target(FocusTarget target)
    {
        Frog.StartFacingPosition(target.GlobalPosition);
    }

    private void Cursor_TargetReleased(FocusTarget target)
    {
        if (!target.IsFocusMax) return;
        AnimateCatchTarget(target);
    }

    private void AnimateCatchTarget(FocusTarget target)
    {
        this.StartCoroutine(Cr, "catch");
        IEnumerator Cr()
        {
            FocusCursor.MoveLock.SetLock("catch", true);
            Cursor.Hide();

            target.FocusCircle.SetVisible(false);
            target.HideGlow();
            target.StopState();
            target.StopMoving();

            Frog.StartFacingPosition(target.GlobalPosition);

            yield return new WaitForSeconds(0.2f);
            yield return Frog.AnimateEatTarget(target);

            Cursor.Show();
            FocusCursor.MoveLock.SetLock("catch", false);

            target.Caught();
        }
    }

    private void HideBackgrounds()
    {
        backgrounds.ForEach(x => x.Hide());
    }

    private void SetBackground(string id)
    {
        HideBackgrounds();
        var background = backgrounds.FirstOrDefault(x => x.Id == id) ?? backgrounds.First();
        background.Show();
    }

    private void SetCoveringEyes(bool is_covering)
    {
        IsCoveringEyes = is_covering;
        Frog.SetCoveringEyes(is_covering);

        if (is_covering)
        {
            GameView.Instance.AnimateVignetteShow(0.25f);
        }
        else
        {
            GameView.Instance.AnimateVignetteHide(0.5f);
        }
    }

    /*
    public virtual void StartEvent_OLD()
    {
        // Disable player
        Player.SetAllLocks(nameof(FocusEvent), true);

        this.StartCoroutine(Cr, "event");
        IEnumerator Cr()
        {
            // Target
            CreateTarget();

            // Frog
            Frog.StartFacingPosition(GlobalPosition);

            // Clear skill checks
            ResetSkillchecks();

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
            FocusEventController.Instance.FocusEventStarted(this);
            this.StartCoroutine(EventCr, "event")
                .SetRunWhilePaused(true);
        }
    }
    */
    /*
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

    private IEnumerator WaitForCatchTarget()
    {
        Frog.SetHandsBack();
        Frog.StartFacingPosition(Target.GlobalPosition);
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
    */
}
