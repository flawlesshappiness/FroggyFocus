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
    public Label ValueLabel;

    [Export]
    public PreviewSubViewport Preview;

    [Export]
    public TextureRect TextureRect;

    [Export]
    public ColorRect ColorRect;

    [Export]
    public AudioStreamPlayer SfxPurchaseSuccess;

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
    }

    public void SetHat(AppearanceHatInfo info)
    {
        var node = info.Prefab.Instantiate<Node3D>();
        Preview.SetPreview(node);

        NameLabel.Text = info.Name;
        ValueLabel.Text = info.Price.ToString();
        current_price = info.Price;

        ColorRect.Hide();
        TextureRect.Show();
    }

    public void SetColor(AppearanceColorInfo info)
    {
        ColorRect.Color = info.Color;

        NameLabel.Text = info.Name;
        ValueLabel.Text = info.Price.ToString();
        current_price = info.Price;

        ColorRect.Show();
        TextureRect.Hide();
    }

    private void Purchase_Pressed()
    {
        if (CanAfford())
        {
            Purchased = true;
            Cancelled = false;

            CurrencyController.Instance.AddValue(CurrencyType.Money, -current_price);
            SfxPurchaseSuccess.Play();
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

    private bool CanAfford()
    {
        return CurrencyController.Instance.GetValue(CurrencyType.Money) >= current_price;
    }
}
