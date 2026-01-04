using Godot;

public partial class MeshAppearanceAttachment : AppearanceAttachment
{
    [Export]
    public MeshInstance3D Mesh;

    private ShaderMaterial primary_material;
    private ShaderMaterial secondary_material;

    public override void _Ready()
    {
        base._Ready();
        InitializeMesh();
    }

    private void InitializeMesh()
    {
        if (Mesh == null) return;

        var count = Mesh.GetSurfaceOverrideMaterialCount();

        primary_material = Mesh.GetActiveMaterial(0).Duplicate() as ShaderMaterial;
        Mesh.SetSurfaceOverrideMaterial(0, primary_material);

        if (count > 1)
        {
            secondary_material = Mesh.GetActiveMaterial(1).Duplicate() as ShaderMaterial;
            Mesh.SetSurfaceOverrideMaterial(1, secondary_material);
        }
    }

    public override void SetPrimaryColor(Color color)
    {
        if (Mesh == null) return;
        primary_material?.SetShaderParameter("albedo", color);
    }

    public override void SetSecondaryColor(Color color)
    {
        if (Mesh == null) return;
        secondary_material?.SetShaderParameter("albedo", color);
    }
}
