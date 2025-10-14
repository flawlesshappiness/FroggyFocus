using Godot;
using System.Collections;
using System.Linq;

public partial class Player : TopDownController
{
    [Export]
    public float MoveSpeed;

    [Export]
    public PlayerInteract PlayerInteract;

    [Export]
    public FrogCharacter Character;

    [Export]
    public ExclamationMark ExclamationMark;

    [Export]
    public QuestionMarkEffect QuestionMark;

    [Export]
    public AudioStreamPlayer3D SfxFocusTargetFound;

    [Export]
    public AudioStreamPlayer3D SfxFocusTargetStarted;

    [Export]
    public GpuParticles3D PsDustStream;

    [Export]
    public GpuParticles3D PsDustJump;

    [Export]
    public GpuParticles3D PsDustLand;

    [Export]
    public Godot.Curve JumpLengthCurve;

    [Export]
    public Godot.Curve JumpHeightCurve;

    public static Player Instance { get; private set; }

    public static MultiLock MovementLock = new();
    public static MultiLock InteractLock = new();
    public static MultiLock FocusEventLock = new();
    public static MultiLock FocusHotSpotLock = new();
    private bool IsCharging { get; set; }
    public bool HasHotspot => FocusHotSpotLock.IsLocked;

    private bool has_focus_target;
    private float jump_charge;
    private bool on_stable_ground;
    private Vector3 respawn_position;

    private Coroutine cr_wait_focus_target;

    public override void _Ready()
    {
        base._Ready();
        Instance = this;

        OnMoveStart += () => MoveChanged(true);
        OnMoveStop += () => MoveChanged(false);
        OnJump += () => JumpChanged(true);
        OnLand += () => JumpChanged(false);

        PlayerInteract.OnHasInteractable += HasInteractables;
        PlayerInteract.OnNoInteractable += NoInteractables;

        FocusEventLock.OnLocked += FocusEventLocked;
        FocusEventLock.OnFree += FocusEventFree;

        EvaluateStableGround();
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        FocusEventLock.OnLocked -= FocusEventLocked;
        FocusEventLock.OnFree -= FocusEventFree;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        Process_Facing();
        Process_Move();
        Process_Interact();
        Process_Jump();
        Process_CameraOffset();
        Process_RespawnPosition();
    }

    private void Process_Move()
    {
        var input = PlayerInput.GetMoveInput();
        if (input.Length() > 0 && !MovementLock.IsLocked && !PlayerInput.Jump.Held)
        {
            Move(input, MoveSpeed);
        }
        else
        {
            Stop();
        }
    }

    private void Process_Facing()
    {
        if (MovementLock.IsLocked) return;
        if (IsJumping) return;

        var input = PlayerInput.GetMoveInput().Normalized();
        var dir = new Vector3(input.X, 0, input.Y);
        if (dir.Length() < 0.1f) return;
        if (dir == Vector3.Zero) return;

        Character.StartFacingDirection(Camera.GlobalBasis * dir);
    }

    private void Process_Jump()
    {
        if (MovementLock.IsLocked) return;
        if (IsJumping) return;

        if (PlayerInput.Jump.Held)
        {
            FocusEventLock.SetLock("jumping", true);

            IsCharging = true;
            jump_charge += GameTime.DeltaTime * 0.5f;
            Character.SetCharging(true);
        }
        else if (PlayerInput.Jump.Released)
        {
            IsCharging = false;
            var t = Mathf.Clamp(jump_charge, 0, 1);
            jump_charge = 0;
            var height = JumpHeightCurve.Sample(t);
            var length = JumpLengthCurve.Sample(t);
            var velocity = Character.Basis * new Vector3(0, height, -length);
            Jump(velocity);
            Character.SetCharging(false);

            on_stable_ground = false;
        }
    }

    private void Process_Interact()
    {
        if (PlayerInput.Interact.Released)
        {
            Interact();
        }
    }

    private void Process_RespawnPosition()
    {
        if (IsJumping) return;
        if (!IsOnFloor()) return;
        if (!on_stable_ground) return;

        respawn_position = GlobalPosition;
    }

    private void Interact()
    {
        if (InteractLock.IsLocked) return;
        if (!IsOnFloor()) return;
        if (PlayerInput.Jump.Held) return;

        var interactable = PlayerInteract.GetInteractable();

        if (has_focus_target)
        {
            StartFocusEvent();
        }
        else if (interactable != null)
        {
            interactable?.Interact();
        }
    }

    private void FocusEventLocked()
    {
        StopWaitForFocusTarget();
    }

    private void FocusEventFree()
    {
        StartWaitForFocusTarget();
    }

    public void SetCameraTarget()
    {
        Camera.Current = true;
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

    public static void SetAllLocks(string key, bool locked)
    {
        MovementLock.SetLock(key, locked);
        InteractLock.SetLock(key, locked);
        FocusEventLock.SetLock(key, locked);
        ThirdPersonCamera.InputLock.SetLock(key, locked);
    }

    private void MoveChanged(bool moving)
    {
        Character.SetMoving(moving);
        FocusEventLock.SetLock("moving", moving);
    }

    private void JumpChanged(bool jumping)
    {
        Character.SetJumping(jumping);

        if (jumping)
        {
            PsDustJump.Emitting = true;
            PlayDustStreamPS(0.4f);
        }
        else
        {
            FocusEventLock.SetLock("jumping", false);
            PsDustLand.Emitting = true;
            StopDustStreamPS();
            EvaluateStableGround();
        }
    }

    private void EvaluateStableGround()
    {
        var closest_position = NavigationServer3D.MapGetClosestPoint(NavigationServer3D.GetMaps().First(), GlobalPosition);
        on_stable_ground = GlobalPosition.DistanceTo(closest_position) < 0.5f;
    }

    public void Respawn()
    {
        var nav_position = NavigationServer3D.MapGetClosestPoint(NavigationServer3D.GetMaps().First(), respawn_position);
        GlobalPosition = nav_position;
        //CameraController.Instance.TeleportCameraToTarget();
    }

    private void PlayDustStreamPS(float duration)
    {
        this.StartCoroutine(Cr, nameof(PlayDustStreamPS));
        IEnumerator Cr()
        {
            PsDustStream.Emitting = true;
            yield return new WaitForSeconds(duration);
            PsDustStream.Emitting = false;
        }
    }

    private void StopDustStreamPS()
    {
        PsDustStream.Emitting = false;
    }

    private void StopWaitForFocusTarget()
    {
        Coroutine.Stop(cr_wait_focus_target);

        if (has_focus_target)
        {
            ExclamationMark.AnimateHide();
            has_focus_target = false;
        }
    }

    private void StartWaitForFocusTarget()
    {
        if (!GameScene.Instance.HasFocusEvent()) return;
        if (!GameScene.Instance.HasFocusEventTargets()) return;

        cr_wait_focus_target = this.StartCoroutine(Cr, nameof(StartWaitForFocusTarget));
        IEnumerator Cr()
        {
            var rng = new RandomNumberGenerator();

            while (true)
            {
                var wait_duration = rng.RandfRange(2f, 8f);
                var wait_end = GameTime.Time + wait_duration;
                while (GameTime.Time < wait_end)
                {
                    if (FocusHotSpotLock.IsLocked) break;
                    yield return null;
                }

                has_focus_target = true;
                ExclamationMark.AnimateShow();
                SfxFocusTargetFound.Play();

                var delay_duration = 2f;
                var delay_end = GameTime.Time + delay_duration;
                while (GameTime.Time < delay_end)
                {
                    while (FocusHotSpotLock.IsLocked)
                    {
                        yield return null;
                    }
                    yield return null;
                }

                ExclamationMark.AnimateHide();
                has_focus_target = false;
            }
        }
    }

    private void StartFocusEvent()
    {
        StopWaitForFocusTarget();

        var id = nameof(StartFocusEvent);
        this.StartCoroutine(Cr, id);
        IEnumerator Cr()
        {
            Player.SetAllLocks(id, true);
            PauseView.ToggleLock.SetLock(id, true);
            SfxFocusTargetStarted.Play();
            yield return ExclamationMark.AnimateBounce();
            Player.SetAllLocks(id, false);
            PauseView.ToggleLock.SetLock(id, false);
            GameScene.Instance.StartFocusEvent();
        }
    }

    private void HasInteractables()
    {
        QuestionMark.AnimateShow();
        FocusEventLock.AddLock("interact");
    }

    private void NoInteractables()
    {
        QuestionMark.AnimateHide();
        FocusEventLock.RemoveLock("interact");
    }
}
