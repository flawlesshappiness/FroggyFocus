using Godot;
using System;

public partial class PurchasePopup : Control
{
    [Export]
    public AnimationPlayer AnimationPlayer;

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
        AnimationPlayer.Play("show");
        CancelButton.GrabFocus();
    }

    private void HidePopup()
    {
        AnimationPlayer.Play("hide");
    }

    public void SetHat(AppearanceHatInfo info)
    {
        var node = info.Prefab.Instantiate<Node3D>();
        Preview.SetPreview(node);

        NameLabel.Text = info.Name;
        ValueLabel.Text = info.Price.ToString();
        current_price = info.Price;
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
