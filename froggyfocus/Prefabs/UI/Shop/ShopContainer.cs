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

    [Export]
    public Control InventoryInfoPanel;

    [Export]
    public InventoryInfoContainer InventoryInfoContainer;

    public override void _Ready()
    {
        base._Ready();
        SellContainer.OnSell += Sell;
        SellContainer.InventoryContainer.OnButtonFocus += SellContainer_ButtonFocus;
        TabContainer.TabChanged += TabContainer_TabChanged;

        InventoryInfoContainer.Clear();
    }

    private void TabContainer_TabChanged(long tab)
    {
        var is_sell = tab == 0;
        InventoryInfoPanel.Visible = is_sell;
        InventoryInfoContainer.Clear();
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

    private void SellContainer_ButtonFocus(InventoryCharacterData data)
    {
        InventoryInfoContainer.SetCharacter(data);
    }
}
