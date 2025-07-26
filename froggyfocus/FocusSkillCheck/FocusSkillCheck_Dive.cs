using Godot;
using System.Collections;

public partial class FocusSkillCheck_Dive : FocusSkillCheck
{
    [Export]
    public Vector2 DistanceRange;

    [Export]
    public AudioStreamPlayer SfxSplash;

    [Export]
    public GpuParticles3D PsRipple;

    [Export]
    public PackedScene SplashParticlePrefab;

    public override void _Ready()
    {
        base._Ready();
    }

    public override void Clear()
    {
        base.Clear();
    }

    protected override IEnumerator Run()
    {
        yield return base.Run();

        CreateSplashPS(Target.GlobalPosition);
        Target.Hide();

        var distance = GetDifficultyRange(DistanceRange);
        var start_position = Target.GlobalPosition;
        var target_position = Target.GetRandomPosition();
        var dir = Target.GlobalPosition.DirectionTo(target_position);
        var end_position = FocusEvent.GlobalPosition + dir * distance;
        Target.GlobalPosition = end_position;

        PsRipple.Emitting = true;
        PsRipple.GlobalPosition = start_position;
        yield return LerpEnumerator.Lerp01(0.5f, f =>
        {
            PsRipple.GlobalPosition = start_position.Lerp(end_position, f);
        });
        PsRipple.Emitting = false;

        CreateSplashPS(Target.GlobalPosition);
        Target.Show();

        yield return new WaitForSeconds(0.25f);
    }

    private void CreateSplashPS(Vector3 position)
    {
        var ps = SplashParticlePrefab.Instantiate<ParticleEffectGroup>();
        ps.SetParent(this);
        ps.Play(destroy: true);
        ps.GlobalPosition = position;

        SfxSplash.Play();
    }
}
