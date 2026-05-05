using Godot;
using Godot.Collections;

public partial class ParticleEffectGroup : EffectGroup
{
    [Export]
    public Array<GpuParticles3D> Particles;

    public override void _Ready()
    {
        base._Ready();
        Particles.ForEach(x => x.Emitting = false);
    }

    protected override void OnPlay()
    {
        base.OnPlay();
        Particles.ForEach(x => x.Emitting = true);
    }

    public override void Stop(bool destroy = false, bool immediate = false)
    {
        Particles.ForEach(x => x.Emitting = false);
        base.Stop(destroy, immediate);
    }
}
