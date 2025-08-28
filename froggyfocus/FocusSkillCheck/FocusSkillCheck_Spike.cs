using Godot;
using System.Collections;

public partial class FocusSkillCheck_Spike : FocusSkillCheck
{
    [Export]
    public Vector2 DelayRange;

    [Export]
    public AnimationPlayer AnimationPlayer;

    [Export]
    public PackedScene PsRipple;

    [Export]
    public AudioStreamPlayer3D SfxCreaks;

    private bool visible;
    private Coroutine cr_spike;

    public override void _Ready()
    {
        base._Ready();
    }

    public override void Clear()
    {
        base.Clear();

        Coroutine.Stop(cr_spike);
        if (visible)
        {
            AnimationPlayer.Play("hide");
            visible = false;
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (IsRunning)
        {
            GlobalPosition = FocusEvent.Cursor.GlobalPosition;
        }
    }

    protected override IEnumerator Run()
    {
        cr_spike = StartSpike();
        yield return null;
    }

    private Coroutine StartSpike()
    {
        var delay_per = DelayRange.Range(Difficulty);

        return this.StartCoroutine(Cr, "spike");
        IEnumerator Cr()
        {
            for (int i = 0; i < 3; i++)
            {
                var ps = ParticleEffectGroup.Instantiate(PsRipple, this);
                ps.Play(destroy: true);

                SfxCreaks.Play();

                yield return new WaitForSeconds(delay_per);
            }

            visible = true;
            var cr_anim = AnimationPlayer.PlayAndWaitForAnimation("show");

            yield return new WaitForSeconds(0.1f); // grace period
            FocusEvent.Cursor.HurtFocusValuePercentage(0.2f);

            yield return cr_anim;

            Clear();
        }
    }
}
