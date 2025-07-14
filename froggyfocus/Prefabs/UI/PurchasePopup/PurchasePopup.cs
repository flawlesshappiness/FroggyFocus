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
    public PackedScene PaintBucketPrefab;

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

    public void SetHat(AppearanceHatInfo info)
    {
        ItemSubViewport.SetPrefab(info.Prefab);

        NameLabel.Text = info.Name;
        PriceControl.SetPrice(info.Price);
        current_price = info.Price;
    }

    public void SetColor(AppearanceColorInfo info)
    {
        var paint_bucket = PaintBucketPrefab.Instantiate<PaintBucket>();
        ItemSubViewport.SetPreview(paint_bucket);
        paint_bucket.SetPaintColor(info.Color);

        NameLabel.Text = info.Name;
        PriceControl.SetPrice(info.Price);
        current_price = info.Price;
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
