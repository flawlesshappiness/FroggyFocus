using Godot;
using System;
using System.Linq;

public partial class PinnedHandInControl : ControlScript
{
    [Export]
    public Label NameLabel;

    [Export]
    public Label RequestLabel;

    [Export]
    public Button UnpinButton;

    public HandInData HandInData { get; private set; }

    public event Action AnyFocusEntered;

    public void Initialize(HandInData data)
    {
        HandInData = data;
        var info = HandInController.Instance.GetInfo(data.Id);
        var request = info.Requests.ToList().GetClamped(data.ClaimCount);
        NameLabel.Text = info.Name;
        RequestLabel.Text = request.GetRequestText();

        UnpinButton.FocusEntered += AnyButton_FocusEntered;
    }

    private void AnyButton_FocusEntered()
    {
        AnyFocusEntered?.Invoke();
    }
}
