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
    public Control InputBlocker;

    private bool animating;

    public override void _Ready()
    {
        base._Ready();
        RegisterDebugActions();

        BackButton.Pressed += BackClicked;
        SellContainer.OnSell += Sell;
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
            Close();
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
}
