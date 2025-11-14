using Godot;
using Godot.Collections;

public partial class CrystalPlatform : Node3D
{
    [Export]
    public MeshInstance3D Mesh;

    [Export]
    public Array<Material> Materials;

    public override void _Ready()
    {
        base._Ready();
        RandomizeColor();
    }

    private void RandomizeColor()
    {
        var mat = Materials.PickRandom();
        Mesh.SetSurfaceOverrideMaterial(0, mat);
    }
}
