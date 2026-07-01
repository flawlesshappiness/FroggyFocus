using Godot;
using System.Collections;
using System.Linq;

public partial class Player : CharacterBody3D
{
    [Export]
    public PackedScene CharacterPrefab;

    [Export]
    public FrogPlayerController Controller;

    [Export]
    public PlayerStableGroundController StableGround;

    [Export]
    public PlayerCharacterEffects Effects;

    [Export]
    public ThirdPersonCamera ThirdPersonCamera;

    public static Player Instance { get; private set; }
    public Camera3D Camera => Controller.CurrentCamera;
    public FrogCharacter Character { get; private set; }

    private static MultiLock input_lock = new();
    private bool has_focus_target;

    private Coroutine cr_look_focus_target;

    public override void _Ready()
    {
        base._Ready();
        Instance = this;

        InitializeCharacter();

        Effects.Interact.OnHasInteractable += HasInteractables;
        Effects.Interact.OnNoInteractable += NoInteractables;

        input_lock.OnLocked += InputLocked;
        input_lock.OnFree += InputFree;

        ThirdPersonCamera.SnapToPosition();

        Controller.OnChargeIndexChanged += Controller_ChargeIndexChanged;
        Controller.OnJump += Controller_Jump;
        Controller.OnSearchingChanged += Controller_SearchingChanged;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        input_lock.OnLocked -= InputLocked;
        input_lock.OnFree -= InputFree;
    }

    private void InitializeCharacter()
    {
        Character = CharacterPrefab.Instantiate<FrogCharacter>();
        Character.SetParent(this);
        Character.ClearPositionAndRotation();
        Effects.SetParent(Character);
        Effects.ClearPositionAndRotation();
        Controller.Character = Character;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        Process_CameraOffset();
    }

    private void Process_CameraOffset()
    {
        if (CameraController.Instance == null) return;
        if (CameraController.Instance.Target != this) return;

        var default_offset = new Vector3(0, 5, 3f);
        var direction_offset = Character.Basis * Vector3.Forward * 2;
        var offset = default_offset + direction_offset;
        CameraController.Instance.Offset = offset;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);
        Input_Pause();
        Input_Interact();
    }

    private void Input_Pause()
    {
        if (input_lock.IsLocked) return;
        if (Controller.IsJumping) return;
        if (Controller.IsCharging) return;
        if (Controller.IsSearching) return;

        if (PlayerInput.Pause.Pressed)
        {
            if (RaceController.Instance.IsStarted)
            {
                RaceController.Instance.OpenPausePopup();
                GetViewport().SetInputAsHandled();
            }
            else
            {
                PauseView.Instance.Open();
                GetViewport().SetInputAsHandled();
            }
        }
    }

    private void Input_Interact()
    {
        if (PlayerInput.Interact.Pressed)
        {
            Interact();
            GetViewport().SetInputAsHandled();
        }
    }

    private void Controller_ChargeIndexChanged(int i)
    {
        var count_max = 3;
        var has_effects = Data.Options.JumpChargeEffectEnabled;
        var t = (float)i / count_max;
        var is_max = i >= count_max;

        if (i > 0 && has_effects)
        {
            Effects.SpawnJumpChargeEffect(t);
        }

        Effects.PsJumpChargeMax.Emitting = is_max && has_effects;
    }

    private void Controller_Jump()
    {
        Effects.PsJumpChargeMax.Emitting = false;
    }

    private void Controller_SearchingChanged(bool is_searching)
    {
        if (is_searching)
        {
            StartLookForFocusTarget();
        }
        else
        {
            StopLookForFocusTarget();
        }

        UpdateInteractables();
    }

    private void Interact()
    {
        if (input_lock.IsLocked) return;

        if (Controller.IsIdle || Controller.IsMoving)
        {
            var interactable = Effects.Interact.GetInteractable();
            interactable?.Interact();
        }
    }

    public void SetCameraTarget()
    {
        Controller.Camera.Current = true;
    }

    public static void SetInputDisabled(string key, bool locked)
    {
        input_lock.SetLock(key, locked);
    }

    public void Respawn()
    {
        Controller.SetRespawning(true);

        TransitionView.Instance.StartTransition(new TransitionSettings
        {
            Type = TransitionType.Color,
            Color = Colors.Black,
            Duration = 0.5f,
            OnTransition = OnTransition
        });

        void OnTransition()
        {
            var position = StableGround.LastPosition;
            var nav_position = NavigationServer3D.MapGetClosestPoint(NavigationServer3D.GetMaps().First(), position);
            GlobalPosition = nav_position.Add(y: -0.5f);
            ThirdPersonCamera.SnapToPosition();

            SetCameraTarget();
            Velocity = Vector3.Zero;
            Controller.SetRespawning(false);
            Effects.SetJumpChargeMaxEmitting(false);
        }
    }

    private void StartLookForFocusTarget()
    {
        if (input_lock.IsLocked) return;
        if (!GameScene.Instance.HasFocusEventTargets()) return;
        if (RaceController.Instance.IsStarted) return;

        var id = "focus_target";
        Character.SetSearching(true);
        StartLowPassFilter();

        var rng = new RandomNumberGenerator();
        cr_look_focus_target = this.StartCoroutine(Cr, id);
        IEnumerator Cr()
        {
            var time_wait = rng.RandfRange(0.5f, 1f);
            GameView.Instance.AnimateVignetteShow(1f);
            AnimateZoom(-1f, 5f);

            yield return new WaitForSeconds(time_wait);

            has_focus_target = true;
            Effects.ExclamationMark.AnimateShow();
        }
    }

    private void StopLookForFocusTarget()
    {
        var id = "focus_target";
        EndLowPassFilter();

        if (cr_look_focus_target == null) return;
        if (!GameScene.Instance.HasFocusEvent()) return;
        if (!GameScene.Instance.HasFocusEventTargets()) return;

        Character.SetSearching(false);

        Coroutine.Stop(cr_look_focus_target);
        cr_look_focus_target = null;
        GameView.Instance.AnimateVignetteHide(0.5f);
        AnimateZoom(0f, 0.5f);

        if (has_focus_target)
        {
            Effects.ExclamationMark.AnimateHide();
            StartFocusEvent();
        }

        has_focus_target = false;
    }

    private void StartFocusEvent()
    {
        var id = nameof(StartFocusEvent);
        this.StartCoroutine(Cr, id);
        IEnumerator Cr()
        {
            Player.SetInputDisabled(id, true);
            Effects.PlayFocusStartSfx();
            yield return Effects.ExclamationMark.AnimateBounce();
            Player.SetInputDisabled(id, false);
            GameScene.Instance.StartFocusEvent();
        }
    }

    private void HasInteractables()
    {
        Effects.QuestionMark.AnimateShow();
        GameView.Instance.InputPrompt.ShowInteract();
    }

    private void NoInteractables()
    {
        Effects.QuestionMark.AnimateHide();
        GameView.Instance.InputPrompt.HidePrompt();
    }

    private void UpdateInteractables()
    {
        var has = Effects.Interact.HasInteractables;
        var search = !Controller.IsSearching;
        var valid = has && search;

        if (valid)
        {
            HasInteractables();
        }
        else
        {
            NoInteractables();
        }
    }

    private void InputLocked()
    {
        Controller.SetInputDisabled(true);
        ThirdPersonCamera.SetInputDisabled(true);
        UpdateInteractables();
    }

    private void InputFree()
    {
        Controller.SetInputDisabled(false);
        ThirdPersonCamera.SetInputDisabled(false);
        UpdateInteractables();
    }

    private void AnimateZoom(float end, float duration)
    {
        this.StartCoroutine(Cr, "zoom");
        IEnumerator Cr()
        {
            var curve = Curves.EaseOutQuad;
            var start = ThirdPersonCamera.ZoomOffset;
            yield return LerpEnumerator.Lerp01(duration, f =>
            {
                var t = curve.Evaluate(f);
                var value = Mathf.Lerp(start, end, t);
                ThirdPersonCamera.SetZoomOffset(value);
            });
        }
    }

    private void StartLowPassFilter()
    {
        AnimateLowPassFilter(500, 1);
    }

    private void EndLowPassFilter()
    {
        AnimateLowPassFilter(20500, 1);
    }

    private void AnimateLowPassFilter(float end, float duration)
    {
        this.StartCoroutine(Cr, "low_pass_filter");
        IEnumerator Cr()
        {
            var bus = AudioBus.Get(AudioBusNames.Master);
            var filter = bus.GetEffect<AudioEffectLowPassFilter>();
            var start = filter.CutoffHz;
            var curve = Curves.EaseOutQuad;
            yield return LerpEnumerator.Lerp01(duration, f =>
            {
                var t = curve.Evaluate(f);
                filter.CutoffHz = Mathf.Lerp(start, end, t);
            });
        }
    }

    public void TeleportToNode(Node3D node)
    {
        if (!IsInstanceValid(node)) return;

        GlobalPosition = node.GlobalPosition;
        Character.RotateToNodeDirectionImmediate(node);
        ThirdPersonCamera.SnapToPosition();
        StableGround.InitializePosition();
    }
}
