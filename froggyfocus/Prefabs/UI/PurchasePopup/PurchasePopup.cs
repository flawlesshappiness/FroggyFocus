using Godot;

public partial class PurchasePopup : PopupControl
{
    [Export]
    public Button CancelButton;

    [Export]
    public Button PurchaseButton;

    [Export]
    public Label NameLabel;

    [Export]
    public PriceControl PriceControl;

    [Export]
    public ItemSubViewport ItemSubViewport;

    [Export]
    public TextureRect TextureRect;

    [Export]
    public AudioStreamPlayer SfxPurchaseFail;

    public bool Purchased { get; private set; }
    public bool Cancelled { get; private set; }

    private int current_price;

    public override void _Ready()
    {
        base._Ready();
        PurchaseButton.Pressed += Purchase_Pressed;
        CancelButton.Pressed += Cancel_Pressed;
        TextureRect.Texture = ItemSubViewport.GetTexture();

        ItemSubViewport.SetCameraFront();
        ItemSubViewport.SetAnimationSpin();
    }

    public void SetAppearanceItem(AppearanceInfo info)
    {
        var item_info = ItemController.Instance.GetInfo(info.Type);
        var shop_info = ShopController.Instance.GetInfo(info.Type);

        ItemSubViewport.SetPrefab(info.Prefab);

        NameLabel.Text = item_info.Name;
        PriceControl.SetPrice(shop_info.Price);
        current_price = shop_info.Price;
    }

    public void SetLocation(LocationInfo info)
    {
        NameLabel.Text = info.Name;
        PriceControl.SetPrice(info.Price);
        current_price = info.Price;

        TextureRect.Texture = info.PreviewImage;
    }

    private void Purchase_Pressed()
    {
        if (Money.CanAfford(current_price))
        {
            Purchased = true;
            Cancelled = false;

            Money.Add(-current_price);
            ClosePopup();
        }
        else
        {
            SfxPurchaseFail.Play();
        }
    }

    public void Cancel()
    {
        Cancel_Pressed();
    }

    private void Cancel_Pressed()
    {
        Cancelled = true;
        Purchased = false;

        ClosePopup();
    }
}
