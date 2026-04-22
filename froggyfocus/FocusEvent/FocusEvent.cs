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
    public Marker3D MainCameraMarker;

    [Export]
    public PackedScene FocusTargetPrefab;

    [Export]
    public FocusFrog Frog;

    [Export]
    public FocusSkillCheck OverrideSkillCheck;

    [Export]
    public Node3D SkillCheckParent;

    [Export]
    public AnimationPlayer AnimationPlayer_Frog;

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
    public bool IsEating { get; private set; }

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
        Cursor.OnDisrupt += Cursor_Disrupt;
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
        Process_MainCamera();
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

    private void Process_MainCamera()
    {
        var speed = 1f;
        if (Cursor.HasTarget)
        {
            var start = MainCameraMarker.GlobalPosition;
            var target = Cursor.CurrentTarget.GlobalPosition;
            var middle = start.Lerp(target, 0.3f).Set(y: start.Y);
            var t = Cursor.CurrentTarget.FocusValue / Cursor.CurrentTarget.FocusMax;
            var y = Mathf.Lerp(start.Y, target.Y, 0.25f);
            var zoom = middle.Set(y: Mathf.Lerp(start.Y, y, t));
            Camera.GlobalPosition = Camera.GlobalPosition.Lerp(zoom, speed * GameTime.DeltaTime);
        }
        else if (!IsEating)
        {
            Camera.GlobalPosition = Camera.GlobalPosition.Lerp(MainCameraMarker.GlobalPosition, speed * GameTime.DeltaTime);
        }
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
        TimerDuration = UpgradeController.Instance.GetCurrentValue(UpgradeType.FocusTime);
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

            yield return new WaitForSeconds(1.0f);

            FocusEventTutorialView.Instance.StartCatchTutorial();
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

        if (HasUncaughtTargets())
        {
            Frog.Character.SetSurprised();
        }

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
        Frog.SetTarget(target);
    }

    private void Cursor_TargetReleased(FocusTarget target)
    {
        Frog.ClearTarget();

        if (!target.IsFocusMax) return;
        AnimateCatchTarget(target);
    }

    private void Cursor_Disrupt()
    {
        Frog.Character.SetSurprised();
    }

    private void AnimateCatchTarget(FocusTarget target)
    {
        this.StartCoroutine(Cr, "catch");
        IEnumerator Cr()
        {
            IsEating = true;

            FocusCursor.MoveLock.SetLock("catch", true);
            Cursor.Hide();

            target.FocusCircle.SetVisible(false);
            target.HideGlow();
            target.StopState();
            target.StopMoving();

            Frog.TurnToTarget(with_cooldown: false);

            yield return new WaitForSeconds(0.2f);
            yield return Frog.Character.AnimateEatTarget(target);

            Cursor.Show();
            FocusCursor.MoveLock.SetLock("catch", false);

            target.Caught();

            IsEating = false;
        }
    }

    private void HideBackgrounds()
    {
        backgrounds.ForEach(x => x.Hide());
    }

    private void SetBackground(string id)
    {
        HideBackgrounds();
        var background = backgrounds.FirstOrDefault(x => x.Id == id || (x.Aliases?.Contains(id) ?? false)) ?? backgrounds.First();
        background.Show();
    }

    private void SetCoveringEyes(bool is_covering)
    {
        IsCoveringEyes = is_covering;
        Frog.Character.SetCoveringEyes(is_covering);

        if (is_covering)
        {
            GameView.Instance.AnimateVignetteShow(0.25f);
        }
        else
        {
            GameView.Instance.AnimateVignetteHide(0.5f);
        }
    }

    private bool HasUncaughtTargets()
    {
        return Targets.Any(x => !x.IsCaught && !x.IsFocusMax);
    }
}
