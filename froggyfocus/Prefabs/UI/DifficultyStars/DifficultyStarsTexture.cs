using Godot;
using Godot.Collections;
using System.Linq;

public partial class DifficultyStarsTexture : TextureRect
{
    [Export]
    public Array<Texture2D> Textures;

    public void SetStars(int count)
    {
        var i = count - 1;
        var texture = Textures.ToList().GetClamped(i);
        Texture = texture;
    }
}
