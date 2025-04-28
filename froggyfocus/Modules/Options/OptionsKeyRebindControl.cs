using Godot;

public partial class OptionsKeyRebindControl : ControlScript
{
    [Export]
    public Label RebindLabel;

    [Export]
    public Label WaitingForInputLabel;

    [Export]
    public Label DuplicateWarningLabel;

    [Export]
    public Button RebindButton;

    [Export]
    public Button ResetButton;

    public void SetWaitingForInput(bool waiting)
    {
        RebindButton.Visible = !waiting;
        WaitingForInputLabel.Visible = waiting;
        ResetButton.Disabled = waiting;
    }
}
