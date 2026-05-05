using Godot;
using System.Collections;

public partial class EffectGroup : Node3D
{
    [Export]
    public bool PlayOnReady;

    [Export]
    public float PlayDelay;

    [Export]
    public float DestroyDelay;

    [Export]
    public Vector3 RotationRandomness;

    public static EffectGroup Instantiate(PackedScene prefab, Node3D parent)
    {
        var ps = prefab.Instantiate<EffectGroup>();
        ps.SetParent(parent);
        ps.ClearPositionAndRotation();
        return ps;
    }

    public override void _Ready()
    {
        base._Ready();

        RandomizeRotation();

        if (PlayOnReady)
        {
            Play();
        }
    }

    public Coroutine Play(bool destroy = false)
    {
        return this.StartCoroutine(Cr, "play");
        IEnumerator Cr()
        {
            if (PlayDelay > 0)
            {
                yield return new WaitForSeconds(PlayDelay);
            }

            OnPlay();

            if (destroy)
            {
                yield return Destroy();
            }
        }
    }

    protected virtual void OnPlay()
    {

    }

    public virtual void Stop(bool destroy = false, bool immediate = false)
    {
        if (destroy)
        {
            Destroy(immediate);
        }
    }

    public Coroutine Destroy(bool immediate = false)
    {
        if (GodotObject.IsInstanceValid(this)) return null;
        if (IsQueuedForDeletion()) return null;
        return this.Destroy(immediate ? 0.0f : DestroyDelay);
    }

    private void RandomizeRotation()
    {
        var rng = new RandomNumberGenerator();
        var x = rng.RandfRange(0, RotationRandomness.X);
        var y = rng.RandfRange(0, RotationRandomness.Y);
        var z = rng.RandfRange(0, RotationRandomness.Z);
        RotationDegrees = new Vector3(x, y, z);
    }
}
