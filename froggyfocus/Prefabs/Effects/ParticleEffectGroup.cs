using Godot;
using Godot.Collections;
using System.Collections;

public partial class ParticleEffectGroup : Node3D
{
    [Export]
    public bool PlayOnReady;

    [Export]
    public float PlayDelay;

    [Export]
    public float DestroyDelay;

    [Export]
    public Array<GpuParticles3D> Particles;

    public override void _Ready()
    {
        base._Ready();

        if (PlayOnReady)
        {
            Play();
        }
    }

    public Coroutine Play(bool destroy = false)
    {
        Particles.ForEach(x => x.Emitting = false);

        return this.StartCoroutine(Cr, "play");
        IEnumerator Cr()
        {
            if (PlayDelay > 0)
            {
                yield return new WaitForSeconds(PlayDelay);
            }

            Particles.ForEach(x => x.Emitting = true);

            if (destroy)
            {
                yield return Destroy();
            }
        }
    }

    public void Stop(bool destroy = false, bool immediate = false)
    {
        Particles.ForEach(x => x.Emitting = false);

        if (destroy)
        {
            Destroy(immediate);
        }
    }

    public Coroutine Destroy(bool immediate = false)
    {
        return this.Destroy(immediate ? 0.0f : DestroyDelay);
    }
}
