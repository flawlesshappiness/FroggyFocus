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
        InitializeAppearanceContainers();

        ShopContainer.BackButton.Pressed += BackClicked;
    }

    private void InitializeAppearanceContainers()
    {
        var containers = this.GetNodesInChildren<AppearanceContainer>();
        foreach (var container in containers)
        {
            container.OnButtonPressed += info => AppearanceButton_Pressed(container, info);
        }
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

    private void AppearanceButton_Pressed(AppearanceContainer container, AppearanceInfo info)
    {
        var focus_button = container.GetButton(info);
        PurchasePopup.SetAppearanceItem(info);

        this.StartCoroutine(Cr, "popup");
        IEnumerator Cr()
        {
            yield return PurchasePopup.WaitForPopup();

            if (PurchasePopup.Purchased)
            {
                Item.MakeOwned(info.Type);
                Data.Game.Save();

                container.UpdateButtons();
                ShopContainer.TabContainer.GetTabBar().GrabFocus();
            }
            else if (PurchasePopup.Cancelled)
            {
                container.GetButton(info).GrabFocus();
            }
        }
    }
}
