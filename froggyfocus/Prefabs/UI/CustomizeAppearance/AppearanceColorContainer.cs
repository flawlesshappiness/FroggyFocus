using Godot;
using System;

public partial class AppearanceColorContainer : Control
{
    [Export]
    public AppearanceColorButton ButtonTemplate;

    public event Action<AppearanceColorType> OnColorPressed;

    public override void _Ready()
    {
        base._Ready();
        InitializeButtons();
    }

    private void InitializeButtons()
    {
        ButtonTemplate.Hide();

        var parent = ButtonTemplate.GetParent();
        foreach (var info in AppearanceColorController.Instance.Collection.Resources)
        {
            var button = ButtonTemplate.Duplicate() as AppearanceColorButton;
            button.SetParent(parent);
            button.Show();
            button.SetColor(info);
            button.Pressed += () => ColorPressed(info.Type);
        }
    }

    private void ColorPressed(AppearanceColorType type)
    {
        OnColorPressed?.Invoke(type);
    }
}
