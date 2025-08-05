using Godot;
using System.Collections;

public partial class FocusSkillCheck_Spike : FocusSkillCheck
{
    [Export]
    public Vector2 DelayRange;

    [Export]
    public AnimationPlayer AnimationPlayer;

    [Export]
    public GpuParticles3D PsRipples;

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
        return this.StartCoroutine(Cr, "spike");
        IEnumerator Cr()
        {
            PsRipples.Emitting = true;
            SfxCreaks.Play();

            var delay = GetDifficultyRange(DelayRange);
            yield return new WaitForSeconds(delay);

            var hurt_amount = FocusEvent.Target.Info.FocusValue * 0.4f;
            FocusEvent.Cursor.HurtFocusValue(hurt_amount);

            visible = true;
            yield return AnimationPlayer.PlayAndWaitForAnimation("show");

            Clear();
        }
    }
}
