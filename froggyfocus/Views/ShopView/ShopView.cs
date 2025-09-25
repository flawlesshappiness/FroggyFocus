using Godot;
using System.Collections;

public partial class ShopView : PanelView
{
    public static ShopView Instance => Get<ShopView>();
    protected override bool IgnoreCreate => false;

    [Export]
    public ShopContainer ShopContainer;

    [Export]
    public PurchasePopup PurchasePopup;

    public override void _Ready()
    {
        base._Ready();
        RegisterDebugActions();

        ShopContainer.BackButton.Pressed += BackClicked;

        ShopContainer.HatsContainer.OnButtonPressed += HatButton_Pressed;
        ShopContainer.ColorContainer.OnButtonPressed += ColorButton_Pressed;
    }

    private void RegisterDebugActions()
    {
        var category = "SHOP";

        Debug.RegisterAction(new DebugAction
        {
            Category = category,
            Text = "Show",
            Action = v => { v.Close(); Show(); }
        });
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (Input.IsActionJustReleased("ui_cancel") && IsVisibleInTree())
        {
            if (PurchasePopup.Active)
            {
                PurchasePopup.Cancel();
            }
            else
            {
                Close();
            }
        }
    }

    protected override void GrabFocusAfterOpen()
    {
        ShopContainer.TabContainer.GetTabBar().GrabFocus();
    }

    private void BackClicked()
    {
        Close();
    }

    private void HatButton_Pressed(AppearanceInfo info)
    {
        var focus_button = ShopContainer.HatsContainer.GetButton(info);
        PurchasePopup.SetAppearanceItem(info);

        this.StartCoroutine(Cr, "popup");
        IEnumerator Cr()
        {
            yield return PurchasePopup.WaitForPopup();

            if (PurchasePopup.Purchased)
            {
                Item.MakeOwned(info.Type);
                Data.Game.Save();

                ShopContainer.HatsContainer.UpdateButtons();
                ShopContainer.TabContainer.GetTabBar().GrabFocus();
            }
            else if (PurchasePopup.Cancelled)
            {
                ShopContainer.HatsContainer.GetButton(info).GrabFocus();
            }
        }
    }

    private void ColorButton_Pressed(AppearanceInfo info)
    {
        var focus_button = ShopContainer.ColorContainer.GetButton(info);
        PurchasePopup.SetAppearanceItem(info);

        this.StartCoroutine(Cr, "popup");
        IEnumerator Cr()
        {
            yield return PurchasePopup.WaitForPopup();

            if (PurchasePopup.Purchased)
            {
                Item.MakeOwned(info.Type);
                Data.Game.Save();

                ShopContainer.ColorContainer.UpdateButtons();
                ShopContainer.TabContainer.GetTabBar().GrabFocus();
            }
            else if (PurchasePopup.Cancelled)
            {
                ShopContainer.ColorContainer.GetButton(info).GrabFocus();
            }
        }
    }
}
