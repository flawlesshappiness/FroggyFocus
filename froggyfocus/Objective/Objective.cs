using System.Linq;

public static class Objective
{
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

    public static void SetLevel(ObjectiveInfo info, int level)
    {
        var data = GetOrCreateData(info.ResourcePath);
        data.Level = level;
        data.Value = 0;
    }
}
