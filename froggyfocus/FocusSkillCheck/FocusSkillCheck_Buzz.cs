using Godot;
using System.Collections;

public partial class FocusSkillCheck_Buzz : FocusSkillCheck
{
    [Export]
    public Vector2I CountRange;

    [Export]
    public Vector2 DelayRange;

    [Export]
    public Vector2 PauseRange;

    [Export]
    public Node3D BuzzEffect;

    [Export]
    public Node3D ChargeEffect;

    [Export]
    public AnimationPlayer AnimationPlayer_BuzzEffect;

    [Export]
    public AnimationPlayer AnimationPlayer_ChargeEffect;

    [Export]
    public AudioStreamPlayer SfxWarn;

    [Export]
    public AudioStreamPlayer SfxBuzz;

    public override void _Process(double delta)
    {
        base._Process(delta);
        Process_BuzzEffect();
    }

    private void Process_BuzzEffect()
    {
        if (Target == null) return;
        BuzzEffect.GlobalPosition = Target.GlobalPosition;
        ChargeEffect.GlobalPosition = Target.GlobalPosition;
    }

    public override void Clear()
    {
        base.Clear();
    }

    protected override IEnumerator Run()
    {
        LocalRun();
        yield return null;
    }

    private Coroutine LocalRun()
    {
        var delay = DelayRange.Range(Difficulty);
        var count = CountRange.Range(Difficulty);

        AnimationPlayer_ChargeEffect.SpeedScale = 1f / delay;

        return this.StartCoroutine(Cr, nameof(LocalRun));
        IEnumerator Cr()
        {
            SfxWarn.Play();
            AnimationPlayer_ChargeEffect.Play("charge");

            yield return new WaitForSeconds(delay);

            for (int i = 0; i < count; i++)
            {
                SfxBuzz.Play();
                FocusEvent.Cursor.HurtFocusValuePercentage(0.1f);

                BuzzEffect.RotationDegrees = Vector3.Up * rng.RandfRange(0, 360f);
                AnimationPlayer_BuzzEffect.Play("buzz");

                var pause = PauseRange.Range(rng.Randf());
                yield return new WaitForSeconds(pause);
            }

            Clear();
        }
    }
}
