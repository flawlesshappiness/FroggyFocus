using System.Linq;

public partial class UpgradeController : ResourceController<UpgradeCollection, UpgradeInfo>
{
    public static UpgradeController Instance => Singleton.Get<UpgradeController>();
    public override string Directory => "Upgrade";

    public UpgradeInfo GetInfo(StatsType type, int level)
    {
        return Collection.Resources.FirstOrDefault(x => x.Type == type && x.Level == level);
    }

    public int GetUpgradeMaxLevel(StatsType type)
    {
        return Collection.Resources.Where(x => x.Type == type)
            .OrderByDescending(x => x.Level)
            .FirstOrDefault()
            ?.Level ?? 0;
    }

    public bool TryPurchaseUpgrade(StatsType type)
    {
        var data = StatsController.Instance.GetOrCreateData(type);
        var next_level = data.Level + 1;
        var upgrade = GetInfo(type, next_level);

        if (upgrade == null) return false;
        if (!CanAffordUpgrade(upgrade)) return false;

        CurrencyController.Instance.AddValue(CurrencyType.Money, -upgrade.Price);
        data.Level = next_level;
        Data.Game.Save();

        return true;
    }

    private bool CanAffordUpgrade(UpgradeInfo info)
    {
        return CurrencyType.Money.Data.Value >= info.Price;
    }
}
