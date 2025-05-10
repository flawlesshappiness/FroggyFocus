using FlawLizArt.Animation.StateMachine;
using Godot;
using System.Collections;

public partial class FrogCharacter : Character
{
    [Export]
    public Node3D Tongue;

    [Export]
    public Marker3D TongueTargetMarker;

    [Export]
    public Marker3D TongueStartMarker;

    [Export]
    public AnimationStateMachine Animation;

    [Export]
    public SoundInfo SfxSwallow;

    private Node3D _attached_target;

    private BoolParameter param_moving = new BoolParameter("moving", false);
    private BoolParameter param_mouth_open = new BoolParameter("mouth_open", false);
    private BoolParameter param_jumping = new BoolParameter("jumping", false);
    private BoolParameter param_charging = new BoolParameter("charging", false);

    public override void _Ready()
    {
        base._Ready();
        InitializeAnimations();
        InitializeTongue();
    }

    private void InitializeTongue()
    {
        Tongue.Scale = new Vector3(1, 1, 0);
        Tongue.Hide();
    }

    private void InitializeAnimations()
    {
        var idle = Animation.CreateAnimation("Armature|idle", true);
        var walking = Animation.CreateAnimation("Armature|walking", true);
        var jump_start = Animation.CreateAnimation("Armature|jump_start", false);
        var jump_end = Animation.CreateAnimation("Armature|jump_end", false);
        var jump_charge = Animation.CreateAnimation("Armature|jump_charge", false);
        var mouth_open = Animation.CreateAnimation("Armature|mouth_open", false);
        var mouth_close = Animation.CreateAnimation("Armature|mouth_close", false);

        Animation.Connect(idle, walking, param_moving.WhenTrue());
        Animation.Connect(walking, idle, param_moving.WhenFalse());

        Animation.Connect(idle, jump_charge, param_charging.WhenTrue());
        Animation.Connect(walking, jump_charge, param_charging.WhenTrue());

        Animation.Connect(idle, mouth_open, param_mouth_open.WhenTrue());
        Animation.Connect(mouth_open, mouth_close, param_mouth_open.WhenFalse());
        Animation.Connect(mouth_open, mouth_close, param_mouth_open.WhenFalse());
        Animation.Connect(mouth_close, idle);

        Animation.Connect(jump_charge, jump_start, param_jumping.WhenTrue());
        Animation.Connect(idle, jump_start, param_jumping.WhenTrue());
        Animation.Connect(walking, jump_start, param_jumping.WhenTrue());
        Animation.Connect(jump_start, jump_end, param_jumping.WhenFalse());
        Animation.Connect(jump_end, idle);

        Animation.Start(idle.Node);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        Process_AttachedTarget();
    }

    public Coroutine AnimateEatTarget(Node3D target)
    {
        return this.StartCoroutine(Cr, nameof(AnimateEatTarget));
        IEnumerator Cr()
        {
            yield return AnimateTongueTowards(target.GlobalPosition);
            AttachToTongue(target);
            yield return AnimateTongueBack();
            SfxSwallow.Play(GlobalPosition);
        }
    }

    public Coroutine AnimateInteract(Node3D target)
    {
        return this.StartCoroutine(Cr, nameof(AnimateEatTarget));
        IEnumerator Cr()
        {
            var empty_length = 2.5f;
            var empty_position = GlobalPosition + Basis * (Vector3.Forward * empty_length + Vector3.Up * 0.25f);
            var position = target == null ? empty_position : target.GlobalPosition;

            if (target != null)
            {
                StartFacingPosition(position);
                yield return new WaitForSeconds(0.25f);
            }

            yield return AnimateTongueTowards(position);
            AnimateTongueBack();
        }
    }

    public Coroutine AnimateTongueTowards(Vector3 position)
    {
        Tongue.GlobalPosition = TongueStartMarker.GlobalPosition;
        Tongue.LookAt(position);
        Tongue.Show();

        var dist = Tongue.GlobalPosition.DistanceTo(position);
        return this.StartCoroutine(Cr, nameof(AnimateTongueTowards));
        IEnumerator Cr()
        {
            param_mouth_open.Set(true);
            yield return new WaitForSeconds(0.2f);

            var start = Tongue.Scale.Z;
            var end = dist;
            yield return LerpEnumerator.Lerp01(0.08f, f =>
            {
                var z = Mathf.Lerp(start, end, f);
                Tongue.Scale = new Vector3(1, 1, z);
            });
        }
    }

    public Coroutine AnimateTongueBack()
    {
        return this.StartCoroutine(Cr, nameof(AnimateTongueBack));
        IEnumerator Cr()
        {
            var start = Tongue.Scale.Z;
            var end = 0;
            yield return LerpEnumerator.Lerp01(0.1f, f =>
            {
                var z = Mathf.Lerp(start, end, f);
                Tongue.Scale = new Vector3(1, 1, z);
            });

            ClearTongueAttachement();
            Tongue.Hide();

            param_mouth_open.Set(false);
            yield return new WaitForSeconds(0.25f);
        }
    }

    public void AttachToTongue(Node3D target)
    {
        if (target == null) return;

        _attached_target = target;
        target.SetParent(this);
    }

    public void ClearTongueAttachement()
    {
        if (_attached_target == null) return;

        _attached_target.Disable();
        _attached_target = null;
    }

    private void Process_AttachedTarget()
    {
        if (_attached_target == null) return;

        _attached_target.GlobalPosition = TongueTargetMarker.GlobalPosition;
    }

    public void SetMoving(bool moving)
    {
        param_moving.Set(moving);
    }

    public void SetJumping(bool jumping)
    {
        param_jumping.Set(jumping);
    }

    public void SetCharging(bool charging)
    {
        param_charging.Set(charging);
    }
}
