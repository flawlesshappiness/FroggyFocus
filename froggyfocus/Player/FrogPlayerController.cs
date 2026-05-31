using Godot;
using System;
using System.Collections.Generic;

public partial class FrogPlayerController : Node3D
{
    [Export]
    public CharacterBody3D Body;

    [Export]
    public Camera3D Camera;

    [Export]
    public Godot.Curve JumpLengthCurve;

    [Export]
    public Godot.Curve JumpHeightCurve;

    public event Action OnJump;
    public event Action OnLand;
    public event Action<int> OnChargeIndexChanged;
    public event Action<bool> OnSearchingChanged;

    public FrogCharacter Character { get; set; }
    public Camera3D OverrideCamera { get; set; }
    public Camera3D CurrentCamera => OverrideCamera ?? Camera;

    public bool IsJumping => state == State.Jump;
    public bool IsFalling => Velocity.Y < 0;
    public bool IsMoving => state == State.Move;
    public bool IsIdle => state == State.Idle;
    public bool IsCharging => state == State.Charge;
    public bool IsRespawning => state == State.Respawn;
    public bool IsSearching => state == State.Search;

    public enum State { Init, Idle, Move, Charge, Jump, Respawn, Search }
    private State state = State.Init;
    private const float Gravity = 20f;

    public bool IsInputDisabled { get; private set; }
    private Vector3 DesiredMoveVelocity { get; set; }
    private Vector3 DesiredJumpVelocity { get; set; }
    private bool HasJumpVelocity => DesiredJumpVelocity.Y > 0;
    private bool HasMoveVelocity => DesiredMoveVelocity.LengthSquared() > 0;
    private int ChargeIndex { get; set; }
    private float ChargeTimeStart { get; set; }
    private const float ChargeDuration = 2.0f;
    private const float MoveSpeed = 2.0f;
    private bool was_respawning;

    private Vector3 Velocity
    {
        get => Body.Velocity;
        set => Body.Velocity = value;
    }

    private bool Grounded => Body.IsOnFloor();

    public override void _Ready()
    {
        base._Ready();
        SetState(State.Idle);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        Process_InputMove();
        Process_InputCharge();
        Process_InputJump();
        Process_InputSearch();
        Process_CharacterFacing();
    }

    private void Process_InputCharge()
    {
        if (IsInputDisabled)
        {
            if (IsCharging)
                SetState(State.Idle);
            ChargeIndex = 0;
            return;
        }

        var is_state = IsIdle || IsMoving;
        if (is_state && PlayerInput.Jump.Held)
        {
            SetState(State.Charge);
        }

        if (Character == null) return;
        if (IsCharging)
        {
            var t = (GameTime.Time - ChargeTimeStart) / ChargeDuration;
            Character.Animation.Animator.SpeedScale = Mathf.Lerp(1.0f, 4.0f, t);

            var t_values = new List<float> { 0.333f, 0.666f, 0.999f };
            if (ChargeIndex < t_values.Count)
            {
                if (t > t_values[ChargeIndex])
                {
                    ChargeIndex++;
                    OnChargeIndexChanged?.Invoke(ChargeIndex);
                }
            }
        }
        else
        {
            Character.Animation.Animator.SpeedScale = 1.0f;
        }
    }

    private void Process_InputJump()
    {
        if (IsCharging && !PlayerInput.Jump.Held)
        {
            Jump();
        }
    }

    private void Process_InputMove()
    {
        if (IsInputDisabled)
        {
            DesiredMoveVelocity = Vector3.Zero;
        }
        else
        {
            var input = PlayerInput.GetMoveInput();
            Vector3 direction = CurrentCamera.GlobalBasis * (new Vector3(input.X, 0, input.Y)).Normalized();
            DesiredMoveVelocity = direction * MoveSpeed;
        }

        if (IsIdle && HasMoveVelocity)
        {
            SetState(State.Move);
        }
        else if (IsMoving && !HasMoveVelocity)
        {
            SetState(State.Idle);
        }
    }

    private void Process_CharacterFacing()
    {
        if (Character == null) return;

        if (IsMoving || IsCharging)
        {
            var dir = HasMoveVelocity ? DesiredMoveVelocity : Character.Basis * Vector3.Forward;
            Character.StartFacingDirection(dir);
        }
    }

    private void Process_InputSearch()
    {
        if (IsInputDisabled) return;
        if (RaceController.Instance.IsStarted) return;

        if (PlayerInput.Focus.Pressed)
        {
            if (IsIdle || IsMoving)
            {
                SetSearching(true);
            }
        }
        else if (PlayerInput.Focus.Released)
        {
            SetSearching(false);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        var fdelta = Convert.ToSingle(delta);
        PhysicsProcess_Move(fdelta);
    }

    private void PhysicsProcess_Move(float delta)
    {
        if (IsRespawning) return;

        Vector3 velocity = Velocity;

        if (Grounded)
        {
            was_respawning = false;
            if (IsIdle)
            {
                velocity = Vector3.Zero;
            }
            else if (IsCharging)
            {
                velocity = Vector3.Zero;
            }
            else if (IsSearching)
            {
                velocity = Vector3.Zero;
            }
            else if (IsMoving)
            {
                velocity.X = DesiredMoveVelocity.X;
                velocity.Z = DesiredMoveVelocity.Z;
            }
            else if (IsJumping)
            {
                if (HasJumpVelocity)
                {
                    velocity = DesiredJumpVelocity;
                }
                else
                {
                    Land();
                    velocity = Vector3.Zero;
                }
            }
        }
        else
        {
            if (!IsJumping && !was_respawning)
            {
                SetState(State.Jump);
                DesiredJumpVelocity = velocity;
            }

            DesiredJumpVelocity = DesiredJumpVelocity.Set(y: 0);
            velocity.X = Mathf.Max(Velocity.X, DesiredJumpVelocity.X);
            velocity.Y -= Gravity * delta;
        }

        Velocity = velocity;
        Body.MoveAndSlide();
    }

    private void Jump()
    {
        SetState(State.Jump);
        DesiredJumpVelocity = GetJumpVelocity();
        PlayJumpSfx();
        OnJump?.Invoke();
    }

    private void Land()
    {
        SetState(State.Idle);
        PlayLandSfx();
        OnLand?.Invoke();
    }

    public void SetState(State state)
    {
        if (this.state == state) return;
        this.state = state;

        if (state == State.Idle)
        {
            DesiredJumpVelocity = Vector3.Zero;
        }
        else if (state == State.Charge)
        {
            ChargeIndex = 0;
            ChargeTimeStart = GameTime.Time;
        }

        UpdateCharacterStates();
    }

    public void SetRespawning(bool respawning)
    {
        was_respawning = respawning || was_respawning;
        SetState(respawning ? State.Respawn : State.Idle);
    }

    public void SetSearching(bool searching)
    {
        SetState(searching ? State.Search : State.Idle);
        OnSearchingChanged?.Invoke(searching);
    }

    public void SetInputDisabled(bool disabled)
    {
        IsInputDisabled = disabled;
    }

    private void UpdateCharacterStates()
    {
        if (Character == null) return;

        Character.SetMoving(IsMoving);
        Character.SetCharging(IsCharging);
        Character.SetJumping(IsJumping);
        Character.SetSearching(IsSearching);
    }

    private void ClearChargeState()
    {
        if (state != State.Charge) return;
        SetState(State.Idle);
    }

    private Vector3 GetJumpVelocity()
    {
        var t = (GameTime.Time - ChargeTimeStart) / ChargeDuration;
        var height = JumpHeightCurve.Sample(t);
        var length = JumpLengthCurve.Sample(t);
        var velocity = Character.Basis * new Vector3(0, height, -length);
        return velocity;
    }

    private void PlayJumpSfx()
    {
        if (Character == null) return;
        Character.MoveSounds.PlayJump();
    }

    private void PlayLandSfx()
    {
        if (Character == null) return;
        Character.MoveSounds.PlayLand();
    }
}
