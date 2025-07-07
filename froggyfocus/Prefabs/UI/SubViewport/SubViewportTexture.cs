using Godot;

public partial class SubViewportTexture : TextureRect
{
    [Export]
    public SubViewport SubViewport;

    public override void _Ready()
    {
        base._Ready();
        Texture = SubViewport.GetTexture();
    }
}
