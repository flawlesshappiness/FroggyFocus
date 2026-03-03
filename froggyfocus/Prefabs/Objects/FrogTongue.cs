using Godot;
using System.Collections;

public partial class FrogTongue : Node3D
{
    [Export]
    public Node3D ScaleNode;

    [Export]
    public Node3D LookNode;

    [Export]
    public Marker3D TongueTargetMarker;

    [Export]
    public AudioStreamPlayer SfxOut;

    [Export]
    public AudioStreamPlayer SfxIn;

    private Node3D _attached_target;

    public override void _Ready()
    {
        base._Ready();
        ScaleNode.Scale = new Vector3(1, 1, 0.01f);
        Hide();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        Process_AttachedTarget();
    }

    public Coroutine AnimateTongueTowards(Vector3 position)
    {
        LookNode.LookAt(position);
        Show();

        SfxOut.Play();

        var dist = LookNode.GlobalPosition.DistanceTo(position);
        return this.StartCoroutine(Cr, nameof(AnimateTongueTowards));
        IEnumerator Cr()
        {
            var duration = 0.08f;
            var start = 0.01f;
            var end = dist;
            var curve = Curves.EaseInQuad;
            yield return LerpEnumerator.Lerp01(duration, f =>
            {
                var t = curve.Evaluate(f);
                var z = Mathf.Lerp(start, end, t);
                ScaleNode.Scale = new Vector3(1, 1, z);
            });
        }
    }

    public Coroutine AnimateTongueBack()
    {
        return this.StartCoroutine(Cr, nameof(AnimateTongueBack));
        IEnumerator Cr()
        {
            SfxIn.Play();

            var duration = 0.1f;
            var start = ScaleNode.Scale.Z;
            var end = 0.01f;
            var curve = Curves.EaseInQuad;
            yield return LerpEnumerator.Lerp01(duration, f =>
            {
                var t = curve.Evaluate(f);
                var z = Mathf.Lerp(start, end, f);
                ScaleNode.Scale = new Vector3(1, 1, z);
            });

            ClearTongueAttachement();
            Hide();
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

        _attached_target.Hide();
        _attached_target = null;
    }

    private void Process_AttachedTarget()
    {
        if (_attached_target == null) return;
        _attached_target.GlobalPosition = TongueTargetMarker.GlobalPosition;
    }
}
