using Godot;
using System;
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
    public Label VariationLabel;

    [Export]
    public DifficultyStarsTexture DifficultyStars;

    [Export]
    public TextureRect TextureRect;

    [Export]
    public ItemSubViewport ItemSubViewport;

    [Export]
    public Button BackButton;

    public event Action BackPressed;

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
        var stats = StatsController.Instance.GetOrCreateCharacterData(info.ResourcePath);

        NameLabel.Text = info.Name;
        LocationLabel.Text = Tr(info.LocationHint);
        CaughtLabel.Text = $"{stats.CountCaught}";
        DifficultyStars.SetStars(stats.HighestRarity);

        var variations = FocusCharacterController.Instance.Collection.Resources.Where(x => x != info && x.Variation == info.Variation).ToList();
        var variation_text = string.Empty;
        for (int i = 0; i < variations.Count; i++)
        {
            var v = variations[i];
            var v_stats = StatsController.Instance.GetOrCreateCharacterData(v.ResourcePath);
            var caught = v_stats.CountCaught > 0;
            variation_text += i > 0 ? "\n" : string.Empty;
            variation_text += caught ? $"{v.Name}" : "???";
        }
        VariationLabel.Text = variation_text;
        VariationContainer.Visible = variations.Count > 0;

        ItemSubViewport.SetCharacter(info);
    }

    private void BackButton_Pressed()
    {
        BackPressed?.Invoke();
    }
}
