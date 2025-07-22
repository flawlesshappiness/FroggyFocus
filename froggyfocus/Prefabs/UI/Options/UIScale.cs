using Godot;

public partial class UIScale : Control
{
    public override void _Ready()
    {
        base._Ready();
        OptionsContainer.OnUIScaleChanged += UIScaleChanged;
        UIScaleChanged();
    }

    private void UIScaleChanged()
    {
        Scale = Vector2.One * Data.Options.UIScale;
    }
}
