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
}
