using Godot;

public partial class ParticleEffectSpawner : Node3D
{
    [Export]
    public PackedScene Effect;

    public void Spawn()
    {
        ParticleEffectGroup.Instantiate(Effect, this);
    }
}
