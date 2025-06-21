using Godot;

public partial class AppearanceAttachment : Node3D
{
    [Export]
    public MeshInstance3D Mesh;

    private ShaderMaterial primary_material;
    private ShaderMaterial secondary_material;

    public override void _Ready()
    {
        base._Ready();

        var max = Mesh.GetSurfaceOverrideMaterialCount();

        primary_material = Mesh.GetActiveMaterial(0).Duplicate() as ShaderMaterial;
        Mesh.SetSurfaceOverrideMaterial(0, primary_material);

        if (max > 1)
        {
            secondary_material = Mesh.GetActiveMaterial(1).Duplicate() as ShaderMaterial;
            Mesh.SetSurfaceOverrideMaterial(1, secondary_material);
        }
    }

    public virtual void SetPrimaryColor(Color color)
    {
        primary_material?.SetShaderParameter("albedo", color);
    }

    public virtual void SetSecondaryColor(Color color)
    {
        secondary_material?.SetShaderParameter("albedo", color);
    }
}
