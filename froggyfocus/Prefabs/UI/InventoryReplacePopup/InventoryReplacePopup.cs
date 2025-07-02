using Godot;
using System.Collections;

public partial class InventoryReplacePopup : ControlScript
{
    [Export]
    public AnimatedOverlay AnimatedOverlay;

    [Export]
    public AnimatedPanel AnimatedPanel;

    [Export]
    public InventoryContainer InventoryContainer;

    [Export]
    public Control InputBlocker;

    [Export]
    public InventoryPreviewButton PreviewButton;

    [Export]
    public Button DiscardButton;

    private FocusCharacterInfo ItemToAdd;

    private bool action_performed;

    public override void _Ready()
    {
        base._Ready();
        InventoryContainer.OnButtonPressed += InventoryButton_Pressed;
        DiscardButton.Pressed += DiscardButton_Pressed;
        PreviewButton.Pressed += DiscardButton_Pressed;
    }

    public IEnumerator WaitForReplace(FocusCharacterInfo info)
    {
        action_performed = false;
        ItemToAdd = info;

        PreviewButton.SetCharacter(info);

        yield return ShowPopup();
        while (!action_performed) yield return null;
        yield return HidePopup();
    }

    private IEnumerator ShowPopup()
    {
        Show();
        InputBlocker.Show();
        ReleaseFocus();

        InventoryContainer.UpdateButtons();

        AnimatedOverlay.AnimateBehindShow();
        yield return AnimatedPanel.AnimatePopShow();

        PreviewButton.GrabFocus();

        MouseVisibility.Instance.Lock.AddLock(nameof(InventoryReplacePopup));

        InputBlocker.Hide();
    }

    private IEnumerator HidePopup()
    {
        InputBlocker.Show();
        ReleaseFocus();

        MouseVisibility.Instance.Lock.RemoveLock(nameof(InventoryReplacePopup));

        AnimatedOverlay.AnimateBehindHide();
        yield return AnimatedPanel.AnimatePopHide();

        Hide();
        InputBlocker.Hide();
    }

    private void InventoryButton_Pressed(FocusCharacterInfo info)
    {
        var data = InventoryContainer.GetSelectedData();
        InventoryController.Instance.RemoveCharacterData(data);
        InventoryController.Instance.AddCharacter(ItemToAdd);
        Data.Game.Save();

        action_performed = true;
    }

    private void DiscardButton_Pressed()
    {
        action_performed = true;
    }
}
