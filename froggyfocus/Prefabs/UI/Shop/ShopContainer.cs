using Godot;

public partial class ShopContainer : Control
{
    [Export]
    public Button BackButton;

    [Export]
    public TabContainer TabContainer;

    [Export]
    public SellContainer SellContainer;

    [Export]
    public HatsContainer HatsContainer;

    [Export]
    public AppearanceColorContainer ColorContainer;

    public override void _Ready()
    {
        base._Ready();
        SellContainer.OnSell += Sell;
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
