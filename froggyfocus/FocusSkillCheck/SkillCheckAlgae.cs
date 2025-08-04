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

    private bool cleared;
    private RandomNumberGenerator rng = new();

    public void StartConstrict()
    {
        cleared = false;

        this.StartCoroutine(Cr, "constrict");
        IEnumerator Cr()
        {
            SfxConstrict.Play();

            FocusCursor.MoveLock.SetLock(nameof(FocusSkillCheck_Constrict), true);

            AnimationPlayer.Play("show");

            while (!cleared)
            {
                yield return null;
            }

            AnimationPlayer.Play("hide");

            FocusCursor.MoveLock.SetLock(nameof(FocusSkillCheck_Constrict), false);
        }
    }

    public void SetCleared()
    {
        cleared = true;
    }

    public void Shake()
    {
        AnimationPlayer.Play("shake");
        SfxConstrict.Play();

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
