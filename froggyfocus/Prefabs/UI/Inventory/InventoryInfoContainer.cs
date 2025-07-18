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
    public PriceControl PriceControl;

    [Export]
    public Label CaughtLabel;

    public override void _Ready()
    {
        base._Ready();
        PreviewTextureRect.Texture = ItemSubViewport.GetTexture();
        ItemSubViewport.SetCameraInventory();
    }

    public void Clear()
    {
        Hide();
    }

    public void SetCharacter(FocusCharacterInfo info)
    {
        if (info == null)
        {
            Clear();
            return;
        }

        Show();

        ItemSubViewport.SetCharacter(info);

        NameLabel.Text = info.Name;
        PriceControl.SetPrice(info.CurrencyReward);

        var stats = StatsController.Instance.GetOrCreateCharacterData(info.ResourcePath);
        CaughtLabel.Text = stats.CountCaught.ToString();
    }
}
