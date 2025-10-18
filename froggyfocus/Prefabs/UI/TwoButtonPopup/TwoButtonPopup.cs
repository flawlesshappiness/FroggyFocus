using Godot;

public partial class TwoButtonPopup : PopupControl
{
    [Export]
    public Label TextLabel;

    [Export]
    public Button AcceptButton;

    [Export]
    public Button CancelButton;

    public bool Accepted { get; private set; }

    public override void _Ready()
    {
        base._Ready();
        AcceptButton.Pressed += AcceptPressed;
        CancelButton.Pressed += CancelPressed;
    }

    private void AcceptPressed()
    {
        Accepted = true;
        ClosePopup();
    }

    private void CancelPressed()
    {
        Accepted = false;
        ClosePopup();
    }
}
