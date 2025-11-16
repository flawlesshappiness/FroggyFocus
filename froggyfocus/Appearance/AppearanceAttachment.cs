using Godot;

public partial class AppearanceAttachment : Node3D
{
    [Export]
    public ItemType Type;

    [Export]
    public MeshInstance3D Mesh;

    public AppearanceInfo Info { get; private set; }

    private ShaderMaterial primary_material;
    private ShaderMaterial secondary_material;

    public override void _Ready()
    {
        base._Ready();

        InitializeInfo();
        InitializeMesh();
    }

    private void InitializeInfo()
    {
        Info = AppearanceController.Instance.GetInfo(Type);

        if (Info == null)
        {
            Debug.LogError($"{Name}: Failed to get AppearanceInfo for {Type}");
        }
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

    public void SetPrimaryColor(Color color)
    {
        if (Mesh == null) return;
        primary_material?.SetShaderParameter("albedo", color);
    }

    public void SetSecondaryColor(Color color)
    {
        if (Mesh == null) return;
        secondary_material?.SetShaderParameter("albedo", color);
    }

    public void SetDefaultColors()
    {
        SetDefaultPrimaryColor();
        SetDefaultSecondaryColor();
    }

    public void SetDefaultPrimaryColor()
    {
        SetPrimaryColor(AppearanceColorController.Instance.GetColor(Info.DefaultPrimaryColor));
    }

    public void SetDefaultSecondaryColor()
    {
        SetSecondaryColor(AppearanceColorController.Instance.GetColor(Info.DefaultSecondaryColor));
    }
}
