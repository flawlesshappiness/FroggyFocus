using Godot;

public partial class StandardMeshAppearanceAttachment : AppearanceAttachment
{
    [Export]
    public MeshInstance3D Mesh;

    private StandardMaterial3D primary_material;
    private StandardMaterial3D secondary_material;

    public override void _Ready()
    {
        base._Ready();
        InitializeMesh();
    }

    private void InitializeMesh()
    {
        if (Mesh == null) return;

        var count = Mesh.GetSurfaceOverrideMaterialCount();

        primary_material = Mesh.GetActiveMaterial(0).Duplicate() as StandardMaterial3D;
        Mesh.SetSurfaceOverrideMaterial(0, primary_material);

        if (count > 1)
        {
            secondary_material = Mesh.GetActiveMaterial(1).Duplicate() as StandardMaterial3D;
            Mesh.SetSurfaceOverrideMaterial(1, secondary_material);
        }
    }

    public override void SetPrimaryColor(Color color)
    {
        if (Mesh == null) return;
        if (primary_material == null) return;

        primary_material.AlbedoColor = color;
    }

    public override void SetSecondaryColor(Color color)
    {
        if (Mesh == null) return;
        if (secondary_material == null) return;

        secondary_material.AlbedoColor = color;
    }
}
