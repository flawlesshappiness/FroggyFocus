using System.Collections.Generic;
using System.Linq;

public partial class AppearanceController : ResourceController<AppearanceCollection, AppearanceInfo>
{
    public static AppearanceController Instance => Singleton.Get<AppearanceController>();
    public override string Directory => "Appearance";

    public AppearanceInfo GetInfo(ItemType type)
    {
        return Collection.Resources.FirstOrDefault(x => x.Type == type);
    }

    public IEnumerable<AppearanceInfo> GetInfos(ItemCategory category)
    {
        return Collection.Resources.Where(x => x.Category == category);
    }
}
