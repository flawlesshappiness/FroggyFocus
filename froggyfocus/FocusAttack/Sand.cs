using Godot;
using System.Collections;

namespace FlawLizArt.FocusEvent;

public partial class Sand : FocusAttack
{
    [Export]
    public Color FlashColor;

    [Export]
    public GpuParticles3D PsSandAttack;

    [Export]
    public AudioStreamPlayer SfxTelegraph;

    [Export]
    public AudioStreamPlayer SfxAttack;

    private Coroutine cr_run;

    protected override void Started()
    {
        base.Started();
        Run();
    }

    protected override void Stopped()
    {
        base.Stopped();
        Coroutine.Stop(cr_run);
    }

    protected override void Caught()
    {
        base.Caught();
        Coroutine.Stop(cr_run);
    }

    private void Run()
    {
        cr_run = this.StartCoroutine(Cr, "run");
        IEnumerator Cr()
        {
            var cooldown = new Vector2(10f, 20f);
            var cooldown_mul = Mathf.Lerp(1f, 0.7f, Target.Difficulty);
            var telegraph_duration = Mathf.Lerp(2f, 1f, Target.Difficulty);

            yield return new WaitForSeconds(rng.RandfRange(2f, cooldown.Y));
            while (true)
            {
                StartState();

                Target.Character.StartFacingPosition(Target.FocusEvent.Frog.GlobalPosition);
                Target.Animate_Exclamation();
                Target.Animate_SandTelegraph();

                SfxTelegraph.Play();

                yield return new WaitForSeconds(telegraph_duration);

                PsSandAttack.GlobalRotation = PsSandAttack.GlobalRotation.Set(y: Target.Character.GlobalRotation.Y);
                Target.Animate_SandAttack();

                SfxAttack.Play();

                yield return new WaitForSeconds(0.2f);

                if (!IsCoveringEyes)
                {
                    HurtFocusValue(0.1f);
                    DisruptCursorFocus();
                    FocusEventView.Instance.Flash(1f, FlashColor);
                }

                yield return new WaitForSeconds(0.5f);

                EndState();

                yield return new WaitForSeconds(rng.RandfRange(cooldown.X, cooldown.Y) * cooldown_mul);
            }
        }
    }
}
