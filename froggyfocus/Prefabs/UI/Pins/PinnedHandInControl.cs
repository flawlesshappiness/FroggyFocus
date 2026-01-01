using Godot;
using Godot.Collections;
using System;

public partial class PinnedHandInControl : ControlScript
{
    [Export]
    public Label NameLabel;

    [Export]
    public Label LocationLabel;

    [Export]
    public Button UnpinButton;

    [Export]
    public Theme ThemeNormal;

    [Export]
    public Theme ThemeCompleted;

    [Export]
    public Array<InventoryPreviewButton> PreviewButtons;

    public HandInData HandInData { get; set; }
    public HandInInfo HandInInfo { get; set; }

    public event Action AnyFocusEntered;

    public void Initialize(HandInData data)
    {
        HandInInfo = HandInController.Instance.GetInfo(data.Id);
        HandInData = data;
        NameLabel.Text = HandInInfo.Name;

        for (int i = 0; i < PreviewButtons.Count; i++)
        {
            var button = PreviewButtons[i];
            var has_info = i < data.RequestInfos.Count;
            button.Visible = has_info;

            if (!has_info) continue;

            var path = data.RequestInfos[i];
            var character = FocusCharacterController.Instance.GetInfoFromPath(path);
            button.SetCharacter(character);
            button.SetObscured(!InventoryController.Instance.HasCharacter(character));

            button.FocusEntered += AnyButton_FocusEntered;
            button.FocusEntered += () => PreviewButton_FocusEnter(button);
            button.FocusExited += () => PreviewButton_FocusExit(button);
        }

        LocationLabel.Hide();

        UnpinButton.FocusEntered += AnyButton_FocusEntered;
    }

    protected override void OnShow()
    {
        base.OnShow();
        UpdatePreviewButtons();
    }

    private void UpdatePreviewButtons()
    {
        var all_completed = true;

        for (int i = 0; i < PreviewButtons.Count; i++)
        {
            var button = PreviewButtons[i];
            var has_info = i < HandInData.RequestInfos.Count;
            button.Visible = has_info;

            if (!has_info) continue;

            var path = HandInData.RequestInfos[i];
            var character = FocusCharacterController.Instance.GetInfoFromPath(path);
            var has_character = InventoryController.Instance.HasCharacter(character);
            button.SetObscured(!has_character);

            all_completed = all_completed && has_character;
        }

        Theme = all_completed ? ThemeCompleted : ThemeNormal;
    }

    private void PreviewButton_FocusEnter(InventoryPreviewButton button)
    {
        var i = PreviewButtons.IndexOf(button);
        var path = HandInData.RequestInfos[i];
        var info = FocusCharacterController.Instance.GetInfoFromPath(path);
        var has = InventoryController.Instance.HasCharacter(info);

        LocationLabel.Text = has ? info.Name : info.LocationHint;
        LocationLabel.Show();
    }

    private void PreviewButton_FocusExit(InventoryPreviewButton button)
    {
        LocationLabel.Hide();
    }

    private void AnyButton_FocusEntered()
    {
        AnyFocusEntered?.Invoke();
    }
}
