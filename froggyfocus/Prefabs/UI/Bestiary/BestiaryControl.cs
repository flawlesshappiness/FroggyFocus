using Godot;
using System;

public partial class BestiaryControl : MarginContainer
{
    [Export]
    public Button BackButton;

    public event Action OnBack;

    public override void _Ready()
    {
        base._Ready();
        BackButton.Pressed += BackButton_Pressed;
    }

    private void BackButton_Pressed()
    {
        OnBack?.Invoke();
    }

    public Control GetFocusControl()
    {
        return BackButton;
    }
}
