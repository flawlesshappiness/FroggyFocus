using Godot;

public partial class UnlockPopup : PopupControl
{
    [Export]
    public Label TitleLabel;

    [Export]
    public RewardPreview ItemUnlockRewardPreview;

    [Export]
    public Control ItemUnlockControl;

    [Export]
    public RewardPreview ShopExpandRewardPreview;

    [Export]
    public Label ShopExpandNameLabel;

    [Export]
    public PriceControl ShopExpandPriceControl;

    [Export]
    public Control ShopExpandControl;

    [Export]
    public Control InstructionsControl;

    [Export]
    public Button OkButton;

    [Export]
    public AudioStreamPlayer SfxFanfare;

    public override void _Ready()
    {
        base._Ready();
        OkButton.Pressed += OkButton_Pressed;

        ItemUnlockRewardPreview.SubViewport.SetAnimationSpin();
        ShopExpandRewardPreview.SubViewport.SetAnimationSpin();
    }

    public void SetShopExpand()
    {
        TitleLabel.Text = "SHOP EXPANDED";
        InstructionsControl.Show();
        ShopExpandControl.Show();
        ItemUnlockControl.Hide();
    }

    public void SetItemUnlock()
    {
        TitleLabel.Text = "ITEM UNLOCKED";
        InstructionsControl.Hide();
        ShopExpandControl.Hide();
        ItemUnlockControl.Show();
    }

    public void SetHat(AppearanceHatType type)
    {
        var info = AppearanceHatController.Instance.GetInfo(type);
        SetHat(info);
    }

    public void SetHat(AppearanceHatInfo info)
    {
        ItemUnlockRewardPreview.SetHat(info);

        ShopExpandRewardPreview.SetHat(info);
        ShopExpandNameLabel.Text = info.Name;
        ShopExpandPriceControl.SetPrice(info.Price);
    }

    private void OkButton_Pressed()
    {
        ClosePopup();
    }

    protected override void PopupShown()
    {
        base.PopupShown();
        SfxFanfare.Play();
    }
}
