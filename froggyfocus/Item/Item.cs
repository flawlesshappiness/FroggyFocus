using System.Collections.Generic;
using System.Linq;

public static class Item
{
    public static ItemData GetOrCreateData(ItemType type)
    {
        var data = Data.Game.Items.FirstOrDefault(x => x.Type == type);

        if (data == null)
        {
            data = new ItemData
            {
                Type = type
            };

            Data.Game.Items.Add(data);
        }

        return data;
    }

    public static bool IsOwned(ItemType type)
    {
        return GetOrCreateData(type).Owned;
    }

    public static void MakeOwned(ItemType type)
    {
        var data = GetOrCreateData(type);
        data.Owned = true;
    }

    public static void SetOwned(ItemType type, bool owned)
    {
        var data = GetOrCreateData(type);
        data.Owned = owned;
    }

    public static bool IsNoneType(ItemType type)
    {
        var none_types = new List<ItemType> { ItemType.Face_None, ItemType.Hat_None, ItemType.Particles_None };
        return none_types.Contains(type);
    }
}
