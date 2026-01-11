using System.Linq;

public partial class LocationController : ResourceController<LocationCollection, LocationInfo>
{
    public static LocationController Instance => Singleton.Get<LocationController>();
    public override string Directory => "Location";

    protected override void Initialize()
    {
        base.Initialize();
        GameProfileController.Instance.OnGameProfileSelected += ProfileSelected;
        ProfileSelected(Data.Options.Profile ?? 1);
    }

    private void ProfileSelected(int profile)
    {
        if (!Data.Game.LocationsInitialized)
        {
            var data = Location.GetOrCreateData("swamp");
            data.Unlocked = true;

            Data.Game.LocationsInitialized = true;
            Data.Game.Save();
        }
    }

    public LocationInfo GetInfo(string id)
    {
        return Collection.Resources.FirstOrDefault(x => x.Id == id);
    }
}
