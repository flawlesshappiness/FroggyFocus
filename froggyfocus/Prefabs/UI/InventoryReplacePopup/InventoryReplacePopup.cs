using Godot;

public partial class InventoryReplacePopup : PopupControl
{
    [Export]
    public InventoryContainer InventoryContainer;

    [Export]
    public InventoryPreviewButton PreviewButton;

    [Export]
    public Button DiscardButton;

    private FocusCharacterInfo ItemToAdd;

    public override void _Ready()
    {
        base._Ready();
        InventoryContainer.OnButtonPressed += InventoryButton_Pressed;
        DiscardButton.Pressed += DiscardButton_Pressed;
        PreviewButton.Pressed += DiscardButton_Pressed;
    }

    protected override void OnShow()
    {
        base.OnShow();
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

    public void SetCharacter(FocusCharacterInfo info)
    {
        ItemToAdd = info;
        PreviewButton.SetCharacter(info);
        InventoryContainer.UpdateButtons();
    }

    private void InventoryButton_Pressed(InventoryCharacterData data)
    {
        InventoryController.Instance.RemoveCharacterData(data);
        InventoryController.Instance.AddCharacter(ItemToAdd);
        Data.Game.Save();

        ClosePopup();
    }

    private void DiscardButton_Pressed()
    {
        ClosePopup();
    }
}
