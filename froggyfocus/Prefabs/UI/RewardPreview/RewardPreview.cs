using Godot;

public partial class RewardPreview : Control
{
    [Export]
    public TextureRect TextureRect;

    [Export]
    public HatSubViewport HatSubViewport;

    [Export]
    public Label AmountLabel;

    [Export]
    public PackedScene CoinStackPrefab;

    public override void _Ready()
    {
        base._Ready();
        TextureRect.Texture = HatSubViewport.GetTexture();
    }

    public void SetHat(AppearanceHatInfo info)
    {
        HatSubViewport.SetHat(info);
        AmountLabel.Hide();
    }

    public void SetCoinStack(int amount)
    {
        HatSubViewport.SetPrefab(CoinStackPrefab);
        SetAmount(amount);
    }

    public void SetAmount(int amount)
    {
        AmountLabel.Show();
        AmountLabel.Text = $"x{amount}";
    }
}
