using Godot;
using System;

public partial class PurchasePopup : Control
{
    [Export]
    public AnimatedOverlay AnimatedOverlay;

    [Export]
    public AnimatedPanel AnimatedPanel;

    [Export]
    public Button CancelButton;

    [Export]
    public Button PurchaseButton;

    [Export]
    public Label NameLabel;

    [Export]
    public Label ValueLabel;

    [Export]
    public ObjectPreview Preview;

    [Export]
    public TextureRect TextureRect;

    [Export]
    public ColorRect ColorRect;

    [Export]
    public AudioStreamPlayer SfxPurchaseSuccess;

    [Export]
    public AudioStreamPlayer SfxPurchaseFail;

    public Action OnPurchase;
    public Action OnCancel;

    private int current_price;

    public override void _Ready()
    {
        base._Ready();
        PurchaseButton.Pressed += Purchase_Pressed;
        CancelButton.Pressed += Cancel_Pressed;
    }

    public void ShowPopup()
    {
        AnimatedOverlay.AnimateBehindShow();
        AnimatedPanel.AnimatePopShow();
        CancelButton.GrabFocus();
    }

    private void HidePopup()
    {
        AnimatedOverlay.AnimateBehindHide();
        AnimatedPanel.AnimatePopHide();
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
            CurrencyController.Instance.AddValue(CurrencyType.Money, -current_price);
            SfxPurchaseSuccess.Play();
            HidePopup();
            OnPurchase?.Invoke();
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
        HidePopup();
        OnCancel?.Invoke();
    }

    private bool CanAfford()
    {
        return CurrencyController.Instance.GetValue(CurrencyType.Money) >= current_price;
    }
}
