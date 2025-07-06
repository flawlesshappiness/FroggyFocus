using Godot;

public partial class AppearancePreviewButton : ButtonScript
{
    [Export]
    public TextureRect HatTextureRect;

    [Export]
    public HatSubViewport HatSubViewport;

    public bool IsLocked { get; private set; }

    public override void _Ready()
    {
        base._Ready();
        HatTextureRect.Texture = HatSubViewport.GetTexture();
    }

    public void SetLocked(bool locked)
    {
        IsLocked = locked;
        HatTextureRect.Modulate = locked ? Colors.Black.SetA(0.5f) : Colors.White;
    }

    public void SetHat(AppearanceHatInfo info)
    {
        HatSubViewport.SetHat(info);
        HatTextureRect.Show();
    }
}
