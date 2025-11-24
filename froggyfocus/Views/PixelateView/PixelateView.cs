using Godot;

public partial class PixelateView : View
{
    [Export]
    public ColorRect Pixelate;

    private ShaderMaterial material;

    public override void _Ready()
    {
        base._Ready();
        material = Pixelate.Material as ShaderMaterial;

        OptionsController.Instance.OnResolutionChanged += ResolutionChanged;

        ResolutionChanged();

        Show();
    }

    private void ResolutionChanged()
    {
        var resolution = GetWindow().Size;
        material.SetShaderParameter("screen_size", resolution);
    }
}
