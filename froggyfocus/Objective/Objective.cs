using Godot;
using System.Linq;

public static class Objective
{
    public static ObjectiveData GetOrCreateData(ObjectiveInfo info) => GetOrCreateData(info.ResourcePath);
    public static ObjectiveData GetOrCreateData(string path)
    {
        var data = Data.Game.Objectives.FirstOrDefault(x => x.InfoPath == path);
        if (data == null)
        {
            data = new ObjectiveData
            {
                InfoPath = path
            };
            Data.Game.Objectives.Add(data);
        }

        return data;
    }

    public static void AddValue(ObjectiveInfo info, int value)
    {
        var data = GetOrCreateData(info);
        SetValue(info, data.Value + value);
    }

    public static void SetValue(ObjectiveInfo info, int value)
    {
        var data = GetOrCreateData(info);
        var max_value = info.Values[data.Level];
        data.Value = Mathf.Clamp(value, 0, max_value);
    }

    public static void SetLevel(ObjectiveInfo info, int level)
    {
        var data = GetOrCreateData(info);
        data.Level = level;
        data.Value = 0;
    }

    public static bool IsMaxLevel(ObjectiveInfo info)
    {
        var data = GetOrCreateData(info);
        var max_level = info.Values.Count;
        return data.Level >= max_level;
    }

    public static bool IsMaxValue(ObjectiveInfo info)
    {
        var data = GetOrCreateData(info);
        var max_value = info.Values[data.Level];
        return data.Value >= max_value;
    }
}
