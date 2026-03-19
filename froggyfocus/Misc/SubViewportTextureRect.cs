using Godot;

public partial class SubViewportTextureRect : TextureRect
{
    [Export]
    public SubViewport SubViewport;

    public override void _Ready()
    {
        base._Ready();
        Texture = SubViewport.GetTexture();
    }
}
