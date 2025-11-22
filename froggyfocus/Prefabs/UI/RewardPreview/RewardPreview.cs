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

    [Export]
    public PackedScene HiddenPreviewPrefab;

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
        SubViewport.SetAnimationIdle();
        SetAmount(amount);
        SetObscured(false);
    }

    public void SetAmount(int amount)
    {
        AmountLabel.Show();
        AmountLabel.Text = $"x{amount}";
    }

    public void SetHiddenPreview()
    {
        SubViewport.SetPrefab(HiddenPreviewPrefab);
        SubViewport.SetCameraInventory();
        SubViewport.SetAnimationSpin();
        AmountLabel.Hide();
        SetObscured(true);
    }

    public void SetObscured(bool obscured)
    {
        var color = obscured ? Colors.Black.SetA(0.5f) : Colors.White;
        TextureRect.Modulate = color;
    }
}
