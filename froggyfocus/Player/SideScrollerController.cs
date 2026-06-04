using Godot;
using System;
using System.Collections;

public partial class SideScrollerController : CharacterBody2D
{
    [Export]
    public AnimationPlayer AnimationPlayer;

    [Export]
    public Sprite2D Sprite;

    [Export]
    public Godot.Curve JumpLengthCurve;

    [Export]
    public Godot.Curve JumpHeightCurve;

    private Vector2 DesiredJumpVelocity { get; set; }
    private float Gravity { get; set; } = 1000f;
    private bool FacingRight { get; set; } = true;
    private float TimeChargeStart { get; set; }
    private float TimeChargeNext { get; set; }
    private float TimeJumpNext { get; set; }
    private Vector2 GroundPosition { get; set; }

    public enum State { Init, Idle, Charge, Jump, Respawn, Disabled }
    private State state = State.Init;

    public override void _Ready()
    {
        base._Ready();
        GroundPosition = GlobalPosition;
        SetState(State.Idle);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        Process_Facing();
        Process_InputCharge();
        Process_InputJump();
    }

    private void Process_Facing()
    {
        var input = PlayerInput.GetMoveInputAfterDeadzone();
        if (input.Length() > 0.1f)
        {
            SetFacingRight(input.X >= 0);
        }
    }

    private void Process_InputCharge()
    {
        if (GameTime.Time < TimeChargeNext) return;
        if (state == State.Idle && PlayerInput.Jump.Held)
        {
            SetState(State.Charge);
        }
    }

    private void Process_InputJump()
    {
        if (GameTime.Time < TimeJumpNext) return;
        if (state == State.Charge && !PlayerInput.Jump.Held)
        {
            SetState(State.Jump);
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
        if (state == State.Respawn) return;
        if (state == State.Disabled) return;

        Vector2 velocity = Velocity;
        var grounded = IsOnFloor();

        if (state == State.Idle)
        {
            velocity = Vector2.Zero;
        }
        else if (grounded)
        {
            if (state == State.Jump)
            {
                if (DesiredJumpVelocity.Y < 0)
                {
                    GroundPosition = GlobalPosition;
                    velocity = DesiredJumpVelocity;
                }
                else
                {
                    SetState(State.Idle);
                    TimeChargeNext = GameTime.Time + 0.1f;
                    velocity = Vector2.Zero;
                }
            }
            else
            {
                velocity = Vector2.Zero;
            }
        }
        else
        {
            DesiredJumpVelocity = DesiredJumpVelocity.Set(y: 0);
            velocity.X = DesiredJumpVelocity.X;
            velocity.Y += Gravity * (float)delta;
        }

        Velocity = velocity;
        MoveAndSlide();
    }

    public void SetState(State state)
    {
        if (this.state == state) return;
        this.state = state;

        if (state == State.Idle)
        {
            AnimationPlayer.Play("idle");
        }
        else if (state == State.Charge)
        {
            var anim = Data.Options.JumpChargeEffectEnabled ? "charge_levels" : "charge";
            AnimationPlayer.Play(anim);
            TimeChargeStart = GameTime.Time;
            TimeJumpNext = GameTime.Time + 0.1f;
        }
        else if (state == State.Jump)
        {
            AnimationPlayer.Play("jump");
            DesiredJumpVelocity = GetJumpVelocity() * 600f;
        }
    }

    public void SetFacingRight(bool right)
    {
        if (FacingRight == right) return;
        if (state == State.Jump) return;
        FacingRight = right;
        Sprite.FlipH = !right;
    }

    private Vector2 GetJumpVelocity()
    {
        var duration = 0.75f;
        var t = (GameTime.Time - TimeChargeStart) / duration;
        var x = JumpLengthCurve.Sample(t);
        var y = JumpHeightCurve.Sample(t);
        var xmul = FacingRight ? 1f : -1f;
        return new Vector2(x * xmul, -y);
    }

    public void Respawn()
    {
        if (state == State.Respawn) return;

        SetState(State.Respawn);
        this.StartCoroutine(Cr, "respawn");
        IEnumerator Cr()
        {
            yield return AnimationPlayer.PlayAndWaitForAnimation("splash");
            GlobalPosition = GroundPosition;
            SetState(State.Idle);
        }
    }

    public void Disable()
    {
        AnimationPlayer.Play("hide");
        SetState(State.Disabled);
    }
}
