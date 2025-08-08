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

    [Export]
    public DifficultyStarsTexture RarityTexture;

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

    public void SetCharacter(InventoryCharacterData data)
    {
        if (data == null)
        {
            Clear();
            return;
        }

        var info = FocusCharacterController.Instance.GetInfoFromPath(data.InfoPath);
        ItemSubViewport.SetCharacter(info);

        NameLabel.Text = info.Name;
        PriceControl.SetPrice(data.Value);

        var stats = StatsController.Instance.GetOrCreateCharacterData(info.ResourcePath);
        CaughtLabel.Text = stats.CountCaught.ToString();

        RarityTexture.SetStars(data.Stars);

        Show();
    }
}
