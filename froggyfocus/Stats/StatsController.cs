using Godot;
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
        var value = GetStatsValue(type, data.Level);
        return value;
    }

    public float GetStatsValue(StatsType type, int level)
    {
        var array = GetStatsValueArray(type);
        var idx = Mathf.Clamp(level, 0, array.Length);
        var value = array[idx];
        return value;
    }

    public float[] GetStatsValueArray(StatsType type) => type switch
    {
        StatsType.CursorStartValue => StatsValues.CursorStartValue,
        StatsType.CursorTickAmount => StatsValues.CursorTickAmount,
        StatsType.CursorTickDecay => StatsValues.CursorTickDecay,
        StatsType.CursorRadius => StatsValues.CursorRadius,
        StatsType.CursorMoveSpeed => StatsValues.CursorMoveSpeed,
        StatsType.CursorMoveSpeedMultiplierDuringFocus => StatsValues.CursorMoveSpeedMultiplierDuringFocus,
        _ => default,
    };

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
