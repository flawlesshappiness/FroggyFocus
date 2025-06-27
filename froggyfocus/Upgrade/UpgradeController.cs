using System.Linq;

public partial class UpgradeController : ResourceController<UpgradeCollection, UpgradeInfo>
{
    public static UpgradeController Instance => Singleton.Get<UpgradeController>();
    public override string Directory => "Upgrade";

    private int[] DefaultPrices = [100, 500, 1000, 4000, 10000, 18000, 30000, 50000];

    public UpgradeInfo GetInfo(StatsType type)
    {
        return Collection.Resources.FirstOrDefault(x => x.Type == type);
    }

    public int GetMaxLevel(StatsType type)
    {
        return GetInfo(type).Values.Count - 1;
    }

    public bool IsMaxLevel(StatsType type)
    {
        return GetCurrentLevel(type) >= GetMaxLevel(type);
    }

    public int GetCurrentLevel(StatsType type)
    {
        var data = StatsController.Instance.GetOrCreateData(type);
        return data?.Level ?? 0;
    }

    public int GetCurrentPrice(StatsType type)
    {
        var level = GetCurrentLevel(type);
        return GetPrice(type, level + 1);
    }

    public int GetPrice(StatsType type, int level)
    {
        return GetOverridePrice(type, level) ?? GetDefaultPrice(level);
    }

    public int GetDefaultPrice(int level)
    {
        if (DefaultPrices.Length <= level) return 100000;
        return DefaultPrices[level];
    }

    public int? GetOverridePrice(StatsType type, int level)
    {
        var info = GetInfo(type);
        if (info.OverridePrice == null) return null;
        if (info.OverridePrice.Count <= level) return null;

        return info.OverridePrice[level];
    }

    public bool TryPurchaseUpgrade(StatsType type)
    {
        var data = StatsController.Instance.GetOrCreateData(type);
        var next_level = data.Level + 1;
        var price = GetPrice(type, next_level);

        if (!Money.CanAfford(price)) return false;

        CurrencyController.Instance.AddValue(CurrencyType.Money, -price);
        data.Level = next_level;
        Data.Game.Save();

        return true;
    }
}
