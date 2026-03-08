using Godot;

public partial class InventoryInfoContainer : Control
{
    [Export]
    public bool ShowPreview;

    [Export]
    public bool ShowValue;

    [Export]
    public bool ShowCaught;

    [Export]
    public bool ShowRarity;

    [Export]
    public Control PreviewParent;

    [Export]
    public TextureRect PreviewTextureRect;

    [Export]
    public ItemSubViewport ItemSubViewport;

    [Export]
    public Control NameParent;

    [Export]
    public Label NameLabel;

    [Export]
    public Control PriceParent;

    [Export]
    public PriceControl PriceControl;

    [Export]
    public Control CaughtParent;

    [Export]
    public Label CaughtLabel;

    [Export]
    public Control RarityParent;

    [Export]
    public DifficultyStarsTexture RarityTexture;

    public override void _Ready()
    {
        base._Ready();
        PreviewTextureRect.Texture = ItemSubViewport.GetTexture();
        ItemSubViewport.SetCameraInventory();

        PreviewParent.Visible = ShowPreview;
        PriceParent.Visible = ShowValue;
        CaughtParent.Visible = ShowCaught;
        RarityParent.Visible = ShowRarity;
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
