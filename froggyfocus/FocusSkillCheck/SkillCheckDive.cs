using Godot;
using System.Collections;

public partial class SkillCheckDive : FocusSkillCheck
{
    [Export]
    public AudioStreamPlayer SfxSplash;

    [Export]
    public PackedScene SplashParticlePrefab;

    [Export]
    public PackedScene BubblesParticlePrefab;

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

        var position = Target.GetClampedPosition();
        Target.GlobalPosition = position;

        CreateBubblesPS(Target.GlobalPosition);

        yield return new WaitForSeconds(3.0f);

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

    private void CreateBubblesPS(Vector3 position)
    {
        var ps = BubblesParticlePrefab.Instantiate<ParticleEffectGroup>();
        ps.SetParent(this);
        ps.Play(destroy: true);
        ps.GlobalPosition = position;
    }
}
