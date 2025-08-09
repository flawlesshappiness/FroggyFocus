using System.Linq;

public partial class LocationController : ResourceController<LocationCollection, LocationInfo>
{
    public static LocationController Instance => Singleton.Get<LocationController>();
    public override string Directory => "Location";

    public LocationInfo GetInfo(string id)
    {
        return Collection.Resources.FirstOrDefault(x => x.Id == id);
    }
}
