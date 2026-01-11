using Godot;

public partial class InventoryReplacePopup : PopupControl
{
    [Export]
    public InventoryContainer InventoryContainer;

    [Export]
    public InventoryPreviewButton PreviewButton;

    [Export]
    public Button DiscardButton;

    [Export]
    public InventoryInfoContainer InfoContainer;

    private FocusTarget current_target;

    public override void _Ready()
    {
        base._Ready();
        InventoryContainer.OnButtonPressed += InventoryButton_Pressed;
        InventoryContainer.OnButtonFocus += InventoryButton_Focus;
        DiscardButton.Pressed += DiscardButton_Pressed;
        PreviewButton.Pressed += DiscardButton_Pressed;
    }

    protected override void OnShow()
    {
        base.OnShow();
        InfoContainer.Clear();
        SetLocks(true);
    }

    protected override void OnHide()
    {
        base.OnHide();
        SetLocks(false);
    }

    private void SetLocks(bool locked)
    {
        var id = nameof(InventoryReplacePopup);
        MouseVisibility.Instance.Lock.SetLock(id, locked);
    }

    public void SetTarget(FocusTarget target)
    {
        current_target = target;
        PreviewButton.SetCharacter(target.Info);
        InventoryContainer.UpdateButtons();
    }

    private void InventoryButton_Pressed(InventoryCharacterData data)
    {
        InventoryController.Instance.RemoveCharacterData(data);
        InventoryController.Instance.AddCharacter(current_target.CharacterData);
        Data.Game.Save();

        ClosePopup();
    }

    private void InventoryButton_Focus(InventoryCharacterData data)
    {
        InfoContainer.SetCharacter(data);
    }

    private void DiscardButton_Pressed()
    {
        ClosePopup();
    }
}
