using Godot;

public partial class ParticleEffectSpawner : Node3D
{
    [Export]
    public PackedScene Effect;

    public void Spawn()
    {
        var node = Effect.Instantiate<ParticleEffectGroup>();
        node.SetParent(this);
        node.ClearPositionAndRotation();
    }
}
