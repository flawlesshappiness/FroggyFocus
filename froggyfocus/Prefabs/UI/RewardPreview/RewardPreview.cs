using Godot;

public partial class RewardPreview : Control
{
    [Export]
    public TextureRect TextureRect;

    [Export]
    public ItemSubViewport SubViewport;

    [Export]
    public Label AmountLabel;

    [Export]
    public PackedScene CoinStackPrefab;

    public override void _Ready()
    {
        base._Ready();
        TextureRect.Texture = SubViewport.GetTexture();
    }

    public void SetAppearanceItem(AppearanceInfo info)
    {
        var item = SubViewport.SetPrefab<AppearanceAttachment>(info.Prefab);
        item.SetDefaultColors();
        AmountLabel.Hide();
    }

    public void SetCoinStack(int amount)
    {
        SubViewport.SetPrefab(CoinStackPrefab);
        SetAmount(amount);
    }

    public void SetAmount(int amount)
    {
        AmountLabel.Show();
        AmountLabel.Text = $"x{amount}";
    }
}
