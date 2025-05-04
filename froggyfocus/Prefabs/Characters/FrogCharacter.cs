using Godot;
using System.Collections;

public partial class FrogCharacter : Character
{
    [Export]
    public Node3D Tongue;

    [Export]
    public Node3D TongueTargetMarker;

    [Export]
    public AnimationPlayer AnimationPlayer;

    private Node3D _attached_target;

    public override void _Ready()
    {
        base._Ready();
        Tongue.Scale = new Vector3(1, 1, 0);
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
            yield return AnimateTongueTowards(target);
            AttachToTongue(target);
            yield return AnimateTongueBack();
        }
    }

    public Coroutine AnimateTongueTowards(Node3D target)
    {
        var dist = Tongue.GlobalPosition.DistanceTo(target.GlobalPosition);
        return this.StartCoroutine(Cr, nameof(AnimateTongueTowards));
        IEnumerator Cr()
        {
            yield return AnimationPlayer.PlayAndWaitForAnimation("open");

            var start = Tongue.Scale.Z;
            var end = dist;
            yield return LerpEnumerator.Lerp01(0.1f, f =>
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

            yield return AnimationPlayer.PlayAndWaitForAnimation("close");
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
}
