using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class InventoryControl : ControlScript
{
    [Export]
    public InventoryPreviewButton InventoryButtonTemplate;

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
    private ButtonMap selected_map;
    private List<ButtonMap> maps = new();

    private class ButtonMap
    {
        public Button Button { get; set; }
        public FocusCharacterInfo Info { get; set; }
        public InventoryCharacterData Data { get; set; }
    }

    public override void _Ready()
    {
        base._Ready();

        BackButton.Pressed += BackPressed;
        DiscardButton.Pressed += DiscardPressed;
    }

    protected override void OnShow()
    {
        base.OnShow();
        InitializeInventoryButtons();
    }

    private void InitializeInventoryButtons()
    {
        InventoryButtonTemplate.Hide();

        ClearMaps();

        foreach (var data in Data.Game.Inventory.Characters)
        {
            var info = FocusCharacterController.Instance.GetInfoFromPath(data.InfoPath);
            if (info == null) continue;

            var button = InventoryButtonTemplate.Duplicate() as InventoryPreviewButton;
            button.SetParent(InventoryButtonTemplate.GetParent());
            button.SetCharacter(info);
            button.Show();

            var map = new ButtonMap
            {
                Button = button,
                Info = info,
                Data = data
            };

            button.Pressed += () => InventoryButton_Pressed(map);

            maps.Add(map);
        }

        ClearCharacterInfo();
        if (maps.Count > 0)
        {
            var map = maps.First();
            InventoryButton_Pressed(map);
        }
    }

    public void GrabFocus_InventoryButton()
    {
        var focus = maps.FirstOrDefault()?.Button ?? BackButton;
        focus.GrabFocus();
    }

    private void Clear()
    {
        ClearMaps();
        ClearCharacterInfo();
        ClearPreviewCharacter();
    }

    private void ClearMaps()
    {
        selected_map = null;
        maps.ForEach(x => x.Button.QueueFree());
        maps.Clear();
    }

    private void InventoryButton_Pressed(ButtonMap map)
    {
        selected_map = map;

        SetPreviewCharacter(map.Info);
        SetCharacterInfo(map.Info);
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

        current_preview = info.Scene.Instantiate<FocusCharacter>();
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
        if (selected_map == null) return;

        InventoryController.Instance.RemoveCharacterData(selected_map.Data);
        Data.Game.Save();

        Clear();

        InitializeInventoryButtons();
        GrabFocus_InventoryButton();
    }
}
