using Godot;

public partial class NoiseToImageScene : Node3D
{
    [Export]
    public Sprite3D NoiseSprite;

    private bool initialized;

    private void Initialize()
    {
        var filepath = "res://Assets/FlawLizArt/Textures/" + "generated_noise.png";
        var img = NoiseSprite.Texture.GetImage();
        img.ClearMipmaps();
        img.SavePng(filepath);
        GD.Print("Noise saved to " + filepath);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (!initialized)
        {
            initialized = true;
            Initialize();
        }
    }
}
