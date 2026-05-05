using Godot;

public partial class EffectGroupSpawner : Node3D
{
    [Export]
    public PackedScene EffectGroupPrefab;

    public EffectGroup Spawn()
    {
        return EffectGroup.Instantiate(EffectGroupPrefab, this);
    }
}
