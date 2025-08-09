using Godot;

public partial class ImageButton : ButtonScript
{
    [Export]
    public TextureRect TextureRect;

    public void SetTexture(Texture2D texture)
    {
        TextureRect.Texture = texture;
    }
}
