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

    public void SetAppearanceItem(ItemType type)
    {
        var info = AppearanceController.Instance.GetInfo(type);
        if (info.Category == ItemCategory.Hat || info.Category == ItemCategory.Face)
        {
            SetAppearanceAttachment(info);
        }
        // TODO COLOR?
    }

    public void SetAppearanceAttachment(AppearanceInfo info)
    {
        var item_info = ItemController.Instance.GetInfo(info.Type);
        var shop_info = ShopController.Instance.GetInfo(info.Type);

        ItemUnlockRewardPreview.SetAppearanceItem(info);
        ShopExpandRewardPreview.SetAppearanceItem(info);
        ShopExpandNameLabel.Text = item_info.Name;
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
