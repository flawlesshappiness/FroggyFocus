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
}
