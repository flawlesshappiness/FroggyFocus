using Godot;

public partial class AppearanceColorButton : ButtonScript
{
    [Export]
    public ColorRect ColorRect;

    public void SetColor(AppearanceColorInfo info)
    {
        ColorRect.Color = info.Color;
    }
}
