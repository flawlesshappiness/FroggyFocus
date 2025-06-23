using Godot;
using System.Collections;

public partial class ShopView : View
{
    public static ShopView Instance => Get<ShopView>();

    [Export]
    public AnimationPlayer AnimationPlayer;

    [Export]
    public Button BackButton;

    [Export]
    public TabContainer TabContainer;

    [Export]
    public UpgradeContainer UpgradeContainer;

    [Export]
    public SellContainer SellContainer;

    [Export]
    public HatsContainer HatsContainer;

    [Export]
    public PurchasePopup PurchasePopup;

    [Export]
    public Control InputBlocker;

    private bool animating;
    private bool popup_active;
    private Control popup_back_focus;

    public override void _Ready()
    {
        base._Ready();
        RegisterDebugActions();

        BackButton.Pressed += BackClicked;
        SellContainer.OnSell += Sell;
        HatsContainer.OnButtonPressed += HatButton_Pressed;
        PurchasePopup.OnCancel += PurchasePopup_OnCancel;
    }

    protected override void OnShow()
    {
        base.OnShow();

        Open();

        Player.SetAllLocks(nameof(ShopView), true);
        MouseVisibility.Show(nameof(ShopView));
    }

    protected override void OnHide()
    {
        base.OnHide();
        Player.SetAllLocks(nameof(ShopView), false);
        MouseVisibility.Hide(nameof(ShopView));
    }

    private void RegisterDebugActions()
    {
        var category = nameof(ShopView);

        Debug.RegisterAction(new DebugAction
        {
            Category = category,
            Text = "Show",
            Action = v => { v.Close(); Show(); }
        });

        Debug.RegisterAction(new DebugAction
        {
            Category = category,
            Text = "Hide",
            Action = v => { v.Close(); Close(); }
        });
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (Input.IsActionJustReleased("ui_cancel") && IsVisibleInTree())
        {
            if (popup_active)
            {
                PurchasePopup.Cancel();
            }
            else
            {
                Close();
            }
        }
    }

    private void Open()
    {
        if (animating) return;
        animating = true;

        this.StartCoroutine(Cr, "animate");
        IEnumerator Cr()
        {
            InputBlocker.Show();
            yield return AnimationPlayer.PlayAndWaitForAnimation("show");
            InputBlocker.Hide();

            TabContainer.GetTabBar().GrabFocus();

            animating = false;
        }
    }

    private void Close()
    {
        if (animating) return;
        animating = true;

        this.StartCoroutine(Cr, "animate");
        IEnumerator Cr()
        {
            InputBlocker.Show();
            yield return AnimationPlayer.PlayAndWaitForAnimation("hide");
            InputBlocker.Hide();
            Hide();
            animating = false;
        }
    }

    private void BackClicked()
    {
        Close();
    }

    private void Sell()
    {
        var button = SellContainer.InventoryContainer.GetFirstButton();
        if (button != null)
        {
            button.GrabFocus();
        }
        else
        {
            TabContainer.GetTabBar().GrabFocus();
        }
    }

    private void HatButton_Pressed(AppearanceHatInfo info)
    {
        popup_active = true;

        popup_back_focus = HatsContainer.GetButton(info);
        PurchasePopup.SetHat(info);
        PurchasePopup.ShowPopup();

        PurchasePopup.OnPurchase = () =>
        {
            Data.Game.Appearance.UnlockedHats.Add(info.Type);
            HatsContainer.UpdateButtons();
            TabContainer.GetTabBar().GrabFocus();

            popup_active = false;
        };
    }

    private void PurchasePopup_OnCancel()
    {
        popup_back_focus.GrabFocus();
        popup_active = false;
    }
}
