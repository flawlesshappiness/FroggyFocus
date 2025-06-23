using Godot;
using System;

public partial class SellContainer : ControlScript
{
    [Export]
    public InventoryContainer InventoryContainer;

    [Export]
    public AudioStreamPlayer SfxSell;

    public event Action OnSell;

    public override void _Ready()
    {
        base._Ready();
        InventoryContainer.OnButtonPressed += Button_Pressed;
    }

    protected override void OnShow()
    {
        base.OnShow();
        InventoryContainer.UpdateButtons();
    }

    private void Button_Pressed(FocusCharacterInfo info)
    {
        CurrencyController.Instance.AddValue(CurrencyType.Money, info.CurrencyReward);

        var data = InventoryContainer.GetSelectedData();
        InventoryController.Instance.RemoveCharacterData(data);
        Data.Game.Save();

        InventoryContainer.UpdateButtons();

        SfxSell.Play();

        OnSell?.Invoke();
    }
}
