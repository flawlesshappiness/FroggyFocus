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
    public Node3D PreviewOrigin;

    [Export]
    public Label NameLabel;

    [Export]
    public Label DescriptionLabel;

    [Export]
    public Label ValueLabel;

    [Export]
    public Control ValueContainer;

    public event Action OnBack;

    private Node3D current_preview;

    public override void _Ready()
    {
        base._Ready();

        BackButton.Pressed += BackPressed;
        DiscardButton.Pressed += DiscardPressed;
        InventoryContainer.OnButtonPressed += InventoryButton_Pressed;
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
        ClearCharacterInfo();
        ClearPreviewCharacter();
    }

    private void InventoryButton_Pressed(FocusCharacterInfo info)
    {
        SetPreviewCharacter(info);
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
        DescriptionLabel.Text = info.Description;
        ValueLabel.Text = info.CurrencyReward.ToString();

        ValueContainer.Show();
        DiscardButton.Show();
    }

    private void ClearCharacterInfo()
    {
        NameLabel.Text = string.Empty;
        DescriptionLabel.Text = string.Empty;

        ValueContainer.Hide();
        DiscardButton.Hide();
    }

    private void SetPreviewCharacter(FocusCharacterInfo info)
    {
        ClearPreviewCharacter();

        var character = info.Scene.Instantiate<FocusCharacter>();
        character.Initialize(info);

        current_preview = character;
        current_preview.SetParent(PreviewOrigin);
        current_preview.Position = Vector3.Zero;
        current_preview.Rotation = Vector3.Zero;
    }

    private void ClearPreviewCharacter()
    {
        if (current_preview == null) return;

        current_preview.QueueFree();
        current_preview = null;
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
