using Godot;
using System;

public partial class CreditsContainer : ControlScript
{
    [Export]
    public Button BackButton;

    public event Action OnBackPressed;

    public override void _Ready()
    {
        base._Ready();
        BackButton.Pressed += BackPressed;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);

        if (Input.IsActionJustReleased("ui_cancel") && IsVisibleInTree())
        {
            GetViewport().SetInputAsHandled();
            BackPressed();
        }
    }

    private void BackPressed()
    {
        OnBackPressed?.Invoke();
    }
}
