using Godot;

[Tool]
public partial class BoxDiscard : MeshInstance3D
{
    [Export]
    public Node3D CutoffNode1;

    [Export]
    public Node3D CutoffNode2;

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (CutoffNode1 == null) return;
        if (CutoffNode2 == null) return;

        var mat = GetSurfaceOverrideMaterial(0) as ShaderMaterial;
        if (mat == null) return;

        mat.SetShaderParameter("cutplane1", CutoffNode1.GlobalTransform);
        mat.SetShaderParameter("cutplane2", CutoffNode2.GlobalTransform);
    }
}
