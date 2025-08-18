using System.Linq;

public static class Location
{
    public static LocationData GetOrCreateData(string id)
    {
        var data = Data.Game.Locations.FirstOrDefault(x => x.Id == id);
        if (data == null)
        {
            data = new LocationData { Id = id };
            Data.Game.Locations.Add(data);
        }

        return data;
    }
}
