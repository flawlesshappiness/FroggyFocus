using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class BestiaryEntryControl : Control
{
    [Export]
    public Label NameLabel;

    [Export]
    public Label CaughtLabel;

    [Export]
    public Label LocationLabel;

    [Export]
    public Control VariationContainer;

    [Export]
    public DifficultyStarsTexture DifficultyStars;

    [Export]
    public TextureRect TextureRect;

    [Export]
    public ItemSubViewport ItemSubViewport;

    [Export]
    public Button BackButton;

    [Export]
    public InventoryPreviewButton VariationPreviewButton;

    public event Action BackPressed;

    private List<InventoryPreviewButton> variation_buttons = new();

    public override void _Ready()
    {
        base._Ready();
        ItemSubViewport.SetCameraInventory();
        ItemSubViewport.SetAnimationOscillate();

        BackButton.Pressed += BackButton_Pressed;

        TextureRect.Texture = ItemSubViewport.GetTexture();
    }

    public void Load(FocusCharacterInfo info)
    {
        UpdateText(info);
        UpdateBigPreview(info);

        var stats = StatsController.Instance.GetOrCreateCharacterData(info.ResourcePath);
        DifficultyStars.SetStars(stats.HighestRarity);

        CreateVariationButtons(info);

        var variations = FocusCharacterController.Instance.Collection.Resources.Where(x => x != info && x.Variation == info.Variation).ToList();
        VariationContainer.Visible = variations.Count > 0;
    }

    private void UpdateText(FocusCharacterInfo info)
    {
        var stats = StatsController.Instance.GetOrCreateCharacterData(info.ResourcePath);
        NameLabel.Text = info.Name;
        LocationLabel.Text = Tr(info.LocationHint);
        CaughtLabel.Text = $"{stats.CountCaught}";
    }

    private void UpdateBigPreview(FocusCharacterInfo info)
    {
        ItemSubViewport.SetCharacter(info);
    }

    private void CreateVariationButtons(FocusCharacterInfo info)
    {
        VariationPreviewButton.Hide();

        // Clear
        foreach (var button in variation_buttons)
        {
            button.QueueFree();
        }
        variation_buttons.Clear();

        // Create buttons
        var variations = FocusCharacterController.Instance.Collection.Resources.Where(x => x != info && x.Variation == info.Variation).ToList();
        CreateVariationButton(info);

        foreach (var v_info in variations)
        {
            CreateVariationButton(v_info);
        }
    }

    private void CreateVariationButton(FocusCharacterInfo info)
    {
        var stats = StatsController.Instance.GetOrCreateCharacterData(info.ResourcePath);

        var button = VariationPreviewButton.Duplicate() as InventoryPreviewButton;
        button.SetParent(VariationPreviewButton.GetParent());
        button.Show();
        button.SetCharacter(info);
        button.SetObscured(stats.CountCaught == 0);
        variation_buttons.Add(button);

        if (stats.CountCaught > 0)
        {
            button.Pressed += () =>
            {
                UpdateText(info);
                UpdateBigPreview(info);
            };
        }
    }

    private void BackButton_Pressed()
    {
        BackPressed?.Invoke();
    }
}
