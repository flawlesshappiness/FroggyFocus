using System.Linq;

public partial class StatsController : ResourceController<StatsCollection, StatsInfo>
{
    public override string Directory => "Stats";
    public static StatsController Instance => Singleton.Get<StatsController>();

    public override void _Ready()
    {
        base._Ready();
        RegisterDebugActions();
    }

    private void RegisterDebugActions()
    {
    }

    public StatsInfo GetInfo(StatsType type)
    {
        return Collection.Resources.FirstOrDefault(x => x.Type == type);
    }

    public float GetCurrentStatsValue(StatsType type)
    {
        var data = GetOrCreateData(type);
        return GetStatsValue(type, data.Level);
    }

    public float GetStatsValue(StatsType type, int level)
    {
        var upgrade = UpgradeController.Instance.GetInfo(type, level);
        return upgrade?.Value ?? 0f;
    }

    public StatsData GetOrCreateData(StatsType type)
    {
        var data = Data.Game.Stats.FirstOrDefault(x => x.Type == type);

        if (data == null)
        {
            data = new StatsData { Type = type };
            Data.Game.Stats.Add(data);
        }

        return data;
    }
}
