using Godot;
using System;

public partial class InventoryControl : ControlScript
{
    [Export]
    public InventoryContainer InventoryContainer;

    [Export]
    public Button BackButton;

    [Export]
    public Button DiscardButton;

    [Export]
    public TextureRect PreviewTextureRect;

    [Export]
    public InventorySubViewport InventorySubViewport;

    [Export]
    public Label NameLabel;

    [Export]
    public Label ValueLabel;

    [Export]
    public Control ValueContainer;

    public event Action OnBack;

    public override void _Ready()
    {
        base._Ready();

        BackButton.Pressed += BackPressed;
        DiscardButton.Pressed += DiscardPressed;
        InventoryContainer.OnButtonPressed += InventoryButton_Pressed;

        PreviewTextureRect.Texture = InventorySubViewport.GetTexture();
    }

    protected override void OnShow()
    {
        base.OnShow();

        InventoryContainer.UpdateButtons();
        ClearCharacterInfo();
        InventoryContainer.PressFirstButton();
    }

    public void GrabFocus_InventoryButton()
    {
        var focus = InventoryContainer.GetFirstButton() ?? BackButton;
        focus.GrabFocus();
    }

    private void Clear()
    {
        InventoryContainer.Clear();
        InventorySubViewport.Clear();
        ClearCharacterInfo();
    }

    private void InventoryButton_Pressed(FocusCharacterInfo info)
    {
        InventorySubViewport.SetCharacter(info);
        SetCharacterInfo(info);
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

    private void SetCharacterInfo(FocusCharacterInfo info)
    {
        ClearCharacterInfo();

        NameLabel.Text = info.Name;
        ValueLabel.Text = info.CurrencyReward.ToString();

        ValueContainer.Show();
        DiscardButton.Show();
    }

    private void ClearCharacterInfo()
    {
        NameLabel.Text = string.Empty;

        ValueContainer.Hide();
        DiscardButton.Hide();
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
