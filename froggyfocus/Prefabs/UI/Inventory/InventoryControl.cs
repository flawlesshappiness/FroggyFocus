using Godot;
using System;

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
        InfoContainer.SetCharacter(InventoryController.Instance.GetInfo(0));
    }

    public void GrabFocus_InventoryButton()
    {
        var focus = InventoryContainer.GetFirstButton() ?? BackButton;
        focus.GrabFocus();
    }

    private void Clear()
    {
        InventoryContainer.Clear();
        InfoContainer.Clear();
    }

    private void InventoryButton_Focus(FocusCharacterInfo info)
    {
        InfoContainer.SetCharacter(info);
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
