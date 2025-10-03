using Godot;

public partial class ColorWall : Node3D
{
    [Export]
    public ColorButtonType Type;

    [Export]
    public ShaderMaterial RedMaterial;

    [Export]
    public ShaderMaterial YellowMaterial;

    [Export]
    public ShaderMaterial BlueMaterial;

    [Export]
    public MeshInstance3D MeshInstance;

    [Export]
    public StaticBody3D Collider;

    private ShaderMaterial material;

    public override void _Ready()
    {
        base._Ready();
        SetMaterial(GetMaterial(Type));
        ColorButtonController.Instance.OnTypeChanged += TypeChanged;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        ColorButtonController.Instance.OnTypeChanged -= TypeChanged;
    }

    private void TypeChanged(ColorButtonType type)
    {
        SetOpen(type == Type);
    }

    private void SetOpen(bool open)
    {
        Collider.ProcessMode = open ? ProcessModeEnum.Disabled : ProcessModeEnum.Inherit;
        material.SetShaderParameter("modelOpacity", open ? 0.0f : 1.0f);
    }

    private void SetMaterial(ShaderMaterial mat)
    {
        material = mat.Duplicate() as ShaderMaterial;
        MeshInstance.SetSurfaceOverrideMaterial(0, material);
    }

    public ShaderMaterial GetMaterial(ColorButtonType type) => type switch
    {
        ColorButtonType.Red => RedMaterial,
        ColorButtonType.Yellow => YellowMaterial,
        ColorButtonType.Blue => BlueMaterial,
        _ => RedMaterial
    };
}
