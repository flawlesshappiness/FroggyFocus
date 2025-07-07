using Godot;

public partial class PaintBucket : Node3D
{
    [Export]
    public MeshInstance3D MeshInstance;

    private ShaderMaterial mat_paint;

    public override void _Ready()
    {
        base._Ready();
        InitializeMesh();
    }

    private void InitializeMesh()
    {
        var idx_mat = 1;
        var mesh = MeshInstance.Mesh;
        var mat = mesh.SurfaceGetMaterial(idx_mat);
        mat_paint = mat.Duplicate() as ShaderMaterial;
        MeshInstance.SetSurfaceOverrideMaterial(idx_mat, mat_paint);
    }

    public void SetPaintColor(Color color)
    {
        mat_paint.SetShaderParameter("albedo", color);
    }
}