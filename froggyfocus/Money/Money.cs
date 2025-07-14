using Godot;
using System;

public partial class Money : Node
{
    public static event Action<int> OnMoneyChanged;

    public static int Value => CurrencyController.Instance.GetValue(CurrencyType.Money);

    public override void _Ready()
    {
        base._Ready();
        Boot.OnControllersCreated += ControllersCreated;
    }

    private void ControllersCreated()
    {
        CurrencyController.Instance.OnCurrencyChanged += CurrencyChanged;
    }

    private void CurrencyChanged(CurrencyType type, int value)
    {
        if (type == CurrencyType.Money)
        {
            OnMoneyChanged?.Invoke(value);
        }
    }

    public static bool CanAfford(int price)
    {
        return Value >= price;
    }

    public static void Add(int amount)
    {
        CurrencyController.Instance.AddValue(CurrencyType.Money, amount);
    }
}
