using Godot;
using System;
using System.Linq;

public partial class SellContainer : ControlScript
{
    [Export]
    public InventoryContainer InventoryContainer;

    [Export]
    public Button SellAllButton;

    public event Action OnSell;

    public override void _Ready()
    {
        base._Ready();
        InventoryContainer.OnButtonPressed += Button_Pressed;
        SellAllButton.Pressed += SellAll_Pressed;
    }

    protected override void OnShow()
    {
        base.OnShow();
        InventoryContainer.UpdateButtons();
        UpdateSellAllButton();
    }

    private void Button_Pressed(InventoryCharacterData data)
    {
        Money.Add(data.Value);
        InventoryController.Instance.RemoveCharacterData(data);
        Data.Game.Save();

        InventoryContainer.UpdateButtons();
        UpdateSellAllButton();
        OnSell?.Invoke();
    }

    private void SellAll_Pressed()
    {
        foreach (var data in Data.Game.Inventory.Characters.ToList())
        {
            var info = FocusCharacterController.Instance.GetInfoFromPath(data.InfoPath);
            Money.Add(info.CurrencyReward);
            InventoryController.Instance.RemoveCharacterData(data);
        }

        Data.Game.Save();

        InventoryContainer.UpdateButtons();
        UpdateSellAllButton();
        OnSell?.Invoke();
    }

    private void UpdateSellAllButton()
    {
        var is_empty = Data.Game.Inventory.Characters.Count == 0;
        SellAllButton.Visible = !is_empty;
    }
}
