using Godot;
using System.Collections;

namespace FlawLizArt.FocusEvent;

public partial class CursorFlower : TrapObject
{
    [Export]
    public AnimationPlayer AnimationPlayer;

    [Export]
    public AudioStreamPlayer3D SfxConstrict;

    [Export]
    public PackedScene PsShake;

    private RandomNumberGenerator rng = new();

    protected override void Started()
    {
        base.Started();
        SfxConstrict.Play();
        AnimationPlayer.Play("show");
    }

    private void Destroy()
    {
        this.StartCoroutine(Cr, "destroy");
        IEnumerator Cr()
        {
            yield return AnimationPlayer.PlayAndWaitForAnimation("hide");
            QueueFree();
        }
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (!FocusEvent.IsRunning) return;
        if (Completed) return;

        if (PlayerInput.Interact.Pressed)
        {
            DecreaseCount();
            UpdateSize();
            Shake();
        }
    }

    private void Shake()
    {
        this.StartCoroutine(Cr, "shake");
        IEnumerator Cr()
        {
            AnimateRotate();
            ParticleEffectGroup.Instantiate(PsShake, this);
            SfxConstrict.Play();
            yield return AnimationPlayer.PlayAndWaitForAnimation("shake");

            if (Completed)
            {
                Destroy();
            }
        }
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
}
