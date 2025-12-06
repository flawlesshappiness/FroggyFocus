using Godot;
using System.Linq;

public partial class BestiaryEntryControl : MarginContainer
{
    [Export]
    public Label NameLabel;

    [Export]
    public Label CaughtLabel;

    [Export]
    public Label LocationLabel;

    [Export]
    public Label VariationLabel;

    [Export]
    public DifficultyStarsTexture DifficultyStars;

    [Export]
    public ItemSubViewport ItemSubViewport;

    public override void _Ready()
    {
        base._Ready();
        ItemSubViewport.SetCameraInventory();
        ItemSubViewport.SetAnimationSpin();
    }

    public void Load(FocusCharacterInfo info)
    {
        var stats = StatsController.Instance.GetOrCreateCharacterData(info.ResourcePath);

        NameLabel.Text = info.Name;
        LocationLabel.Text = Tr(info.LocationHint);
        CaughtLabel.Text = $"{stats.CountCaught}";
        DifficultyStars.SetStars(stats.HighestRarity);

        var variations = FocusCharacterController.Instance.Collection.Resources.Where(x => x != info && x.Variation == info.Variation);
        var variation_text = string.Empty;
        foreach (var v in variations)
        {
            variation_text += $"{v.Name}\n";
        }
        variation_text.TrimEnd('\n');
        VariationLabel.Text = variation_text;

        ItemSubViewport.SetCharacter(info);
    }
}
