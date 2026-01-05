using Godot;

public partial class ParticlesAppearanceAttachment : AppearanceAttachment
{
    [Export]
    public GpuParticles3D Particles;

    private StandardMaterial3D material;

    public override void _Ready()
    {
        base._Ready();
        InitializeMaterial();
    }

    private void InitializeMaterial()
    {
        material = Particles.DrawPass1.SurfaceGetMaterial(0).Duplicate() as StandardMaterial3D;
        Particles.DrawPass1.SurfaceSetMaterial(0, material);
    }

    public override void SetPrimaryColor(Color color)
    {
        material.AlbedoColor = color;
    }

    public override void SetSecondaryColor(Color color)
    {
        base.SetSecondaryColor(color);
    }
}
