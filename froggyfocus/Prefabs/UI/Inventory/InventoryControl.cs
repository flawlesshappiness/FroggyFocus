using Godot;
using System;
using System.Linq;

public partial class InventoryControl : ControlScript
{
    [Export]
    public InventoryContainer InventoryContainer;

    [Export]
    public Button BackButton;

    [Export]
    public InventoryInfoContainer InfoContainer;

    public event Action OnBack;

    public override void _Ready()
    {
        base._Ready();

        BackButton.Pressed += BackPressed;
        InventoryContainer.OnButtonFocus += InventoryButton_Focus;
    }

    protected override void OnShow()
    {
        base.OnShow();

        Clear();
        InventoryContainer.UpdateButtons();
        InfoContainer.SetCharacter(Data.Game.Inventory.Characters.FirstOrDefault());
    }

    public Control GetFocusControl()
    {
        return InventoryContainer.GetFirstButton() ?? BackButton;
    }

    public void GrabFocus_InventoryButton()
    {
        var focus = GetFocusControl();
        focus.GrabFocus();
    }

    private void Clear()
    {
        InventoryContainer.Clear();
        InfoContainer.Clear();
    }

    private void InventoryButton_Focus(InventoryCharacterData data)
    {
        InfoContainer.SetCharacter(data);
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (Input.IsActionJustReleased("ui_cancel") && IsVisibleInTree())
        {
            BackPressed();
        }
    }

    private void BackPressed()
    {
        OnBack?.Invoke();
    }

    private void DiscardPressed()
    {
        var data = InventoryContainer.GetSelectedData();
        if (data == null) return;

        InventoryController.Instance.RemoveCharacterData(data);
        Data.Game.Save();

        Clear();

        InventoryContainer.UpdateButtons();
        GrabFocus_InventoryButton();
    }
}
