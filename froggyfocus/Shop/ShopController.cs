using System.Collections.Generic;
using System.Linq;

public partial class ShopController : ResourceController<ShopItemCollection, ShopItemInfo>
{
    public static ShopController Instance => Singleton.Get<ShopController>();
    public override string Directory => "Shop";

    public ShopItemInfo GetInfo(ItemType type)
    {
        return Collection.Resources.FirstOrDefault(x => x.Type == type);
    }

    public IEnumerable<ShopItemInfo> GetInfos(ItemCategory category)
    {
        return Collection.Resources.Where(x => x.Category == category);
    }
}
