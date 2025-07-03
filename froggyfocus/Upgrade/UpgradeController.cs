using Godot;
using System.Linq;

public partial class UpgradeController : ResourceController<UpgradeCollection, UpgradeInfo>
{
    public static UpgradeController Instance => Singleton.Get<UpgradeController>();
    public override string Directory => "Upgrade";

    private int[] DefaultPrices = [100, 500, 1000, 4000, 10000, 18000, 30000, 50000];

    public UpgradeInfo GetInfo(UpgradeType type)
    {
        return Collection.Resources.FirstOrDefault(x => x.Type == type);
    }

    public UpgradeData GetOrCreateData(UpgradeType type)
    {
        var data = Data.Game.Upgrades.FirstOrDefault(x => x.Type == type);

        if (data == null)
        {
            data = new UpgradeData { Type = type };
            Data.Game.Upgrades.Add(data);
        }

        return data;
    }

    public int GetMaxLevel(UpgradeType type)
    {
        return GetInfo(type).Values.Count - 1;
    }

    public bool IsMaxLevel(UpgradeType type)
    {
        return GetCurrentLevel(type) >= GetMaxLevel(type);
    }

    public int GetCurrentLevel(UpgradeType type)
    {
        var data = GetOrCreateData(type);
        return data?.Level ?? 0;
    }

    public float GetCurrentValue(UpgradeType type)
    {
        var data = GetOrCreateData(type);
        return GetValue(type, data.Level);
    }

    public float GetValue(UpgradeType type, int level)
    {
        var info = GetInfo(type);
        var idx = Mathf.Clamp(level, 0, info.Values.Count - 1);
        return info.Values[idx];
    }

    public int GetCurrentPrice(UpgradeType type)
    {
        var level = GetCurrentLevel(type);
        return GetPrice(type, level + 1);
    }

    public int GetPrice(UpgradeType type, int level)
    {
        return GetOverridePrice(type, level) ?? GetDefaultPrice(level);
    }

    public int GetDefaultPrice(int level)
    {
        if (DefaultPrices.Length <= level) return 100000;
        return DefaultPrices[level];
    }

    public int? GetOverridePrice(UpgradeType type, int level)
    {
        var info = GetInfo(type);
        if (info.OverridePrice == null) return null;
        if (info.OverridePrice.Count <= level) return null;

        return info.OverridePrice[level];
    }

    public bool TryPurchaseUpgrade(UpgradeType type)
    {
        var data = GetOrCreateData(type);
        var next_level = data.Level + 1;
        var price = GetPrice(type, next_level);

        if (!Money.CanAfford(price)) return false;

        CurrencyController.Instance.AddValue(CurrencyType.Money, -price);
        data.Level = next_level;
        Data.Game.Save();

        return true;
    }
}
