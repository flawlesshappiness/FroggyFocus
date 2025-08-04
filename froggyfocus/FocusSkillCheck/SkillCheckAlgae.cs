using Godot;
using System.Collections;

public partial class SkillCheckAlgae : Node3D
{
    [Export]
    public AnimationPlayer AnimationPlayer;

    [Export]
    public Node3D SizeNode;

    [Export]
    public AudioStreamPlayer3D SfxConstrict;

    [Export]
    public PackedScene PsShake;

    private bool visible;
    private RandomNumberGenerator rng = new();

    public void Clear()
    {
        if (visible)
        {
            visible = false;
            AnimationPlayer.Play("hide");
        }
    }

    public void StartConstrict()
    {
        SfxConstrict.Play();
        AnimationPlayer.Play("show");
        visible = true;
    }

    public void Shake()
    {
        AnimationPlayer.Play("shake");
        SfxConstrict.Play();

        ParticleEffectGroup.Instantiate(PsShake, this);

        AnimateRotate();
    }

    private void AnimateRotate()
    {
        this.StartCoroutine(Cr, "rotate");
        IEnumerator Cr()
        {
            var curve = Curves.EaseOutQuad;
            var mul = rng.RandiRange(0, 1) == 0 ? 1 : -1;
            var start = SizeNode.GlobalRotationDegrees;
            var end = start + Vector3.Up * 20 * mul;
            yield return LerpEnumerator.Lerp01(0.2f, f =>
            {
                var t = curve.Evaluate(f);
                SizeNode.GlobalRotationDegrees = start.Lerp(end, t);
            });
        }
    }

    public void UpdateSize(float count, float max)
    {
        var t = 1f - (float)count / max;
        var size = Mathf.Lerp(1f, 0.25f, t);
        SizeNode.Scale = Vector3.One * size;
    }
}
