using Godot;

public partial class InventoryInfoContainer : Control
{
    [Export]
    public TextureRect PreviewTextureRect;

    [Export]
    public ItemSubViewport ItemSubViewport;

    [Export]
    public Label NameLabel;

    [Export]
    public Label ValueLabel;

    [Export]
    public Label CaughtLabel;

    public override void _Ready()
    {
        base._Ready();
        PreviewTextureRect.Texture = ItemSubViewport.GetTexture();
        ItemSubViewport.SetCameraInventory();
    }

    public void SetCharacter(FocusCharacterInfo info)
    {
        ItemSubViewport.SetCharacter(info);

        NameLabel.Text = info.Name;
        ValueLabel.Text = info.CurrencyReward.ToString();

        var stats = StatsController.Instance.GetOrCreateCharacterData(info.ResourcePath);
        CaughtLabel.Text = stats.CountCaught.ToString();
    }
}
