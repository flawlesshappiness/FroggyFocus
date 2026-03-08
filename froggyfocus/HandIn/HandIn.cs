using System.Linq;

public static class HandIn
{
    public static HandInData GetOrCreateData(string id)
    {
        var data = Data.Game.HandIns.FirstOrDefault(x => x.Id == id);
        if (data == null)
        {
            data = new HandInData
            {
                Id = id,
            };

            Data.Game.HandIns.Add(data);
        }

        return data;
    }
}
