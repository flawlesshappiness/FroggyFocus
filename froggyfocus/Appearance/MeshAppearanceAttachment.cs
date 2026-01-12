using Godot;
using Godot.Collections;
using System.Collections.Generic;

public partial class MeshAppearanceAttachment : AppearanceAttachment
{
    [Export]
    public Array<MeshInstance3D> OnlySecondaryMeshes;

    private List<MeshGroup> meshes = new();

    private class MeshGroup
    {
        public MeshInstance3D Mesh { get; set; }
        public ShaderMaterial PrimaryMaterial { get; set; }
        public ShaderMaterial SecondaryMaterial { get; set; }
    }

    public override void _Ready()
    {
        base._Ready();
        InitializeMeshes();
    }

    private void InitializeMeshes()
    {
        var ms = this.GetNodesInChildren<MeshInstance3D>();
        foreach (var mesh in ms)
        {
            var group = new MeshGroup();
            meshes.Add(group);

            var count = mesh.GetSurfaceOverrideMaterialCount();
            var is_only_secondary = OnlySecondaryMeshes?.Contains(mesh) ?? false;

            if (!is_only_secondary)
            {
                group.PrimaryMaterial = mesh.GetActiveMaterial(0).Duplicate() as ShaderMaterial;
                mesh.SetSurfaceOverrideMaterial(0, group.PrimaryMaterial);
            }
            else
            {
                group.SecondaryMaterial = mesh.GetActiveMaterial(0).Duplicate() as ShaderMaterial;
                mesh.SetSurfaceOverrideMaterial(0, group.SecondaryMaterial);
            }

            if (count > 1)
            {
                group.SecondaryMaterial = mesh.GetActiveMaterial(1).Duplicate() as ShaderMaterial;
                mesh.SetSurfaceOverrideMaterial(1, group.SecondaryMaterial);
            }
        }
    }

    public override void SetPrimaryColor(Color color)
    {
        foreach (var mesh in meshes)
        {
            mesh.PrimaryMaterial?.SetShaderParameter("albedo", color);
        }
    }

    public override void SetSecondaryColor(Color color)
    {
        foreach (var mesh in meshes)
        {
            mesh.SecondaryMaterial?.SetShaderParameter("albedo", color);
        }
    }
}
