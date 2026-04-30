using System.Collections.Generic;
using System.Linq;

public partial class ItemController : ResourceController<ItemCollection, ItemInfo>
{
    public static ItemController Instance => Singleton.Get<ItemController>();
    public override string Directory => "Item";

    public override void _Ready()
    {
        base._Ready();
        GameProfileController.Instance.OnGameProfileSelected += GameProfileSelected;

        RegisterDebugActions();
    }

    private void RegisterDebugActions()
    {
        var category = "ITEM";

        Debug.RegisterAction(new DebugAction
        {
            Category = category,
            Text = "Select category",
            Action = ListCategories
        });

        void ListCategories(DebugView v)
        {
            v.SetContent_Search();

            var categories = System.Enum.GetValues(typeof(ItemCategory)).Cast<ItemCategory>();
            foreach (var category in categories)
            {
                var c = category;
                v.ContentSearch.AddItem(category.ToString(), () => ListItemsFromCategory(v, c));
            }

            v.ContentSearch.UpdateButtons();
        }

        void ListItemsFromCategory(DebugView v, ItemCategory category)
        {
            v.SetContent_Search();

            var infos = ItemController.Instance.GetInfos(category);
            foreach (var info in infos)
            {
                v.ContentSearch.AddItem(info.Type.ToString(), () => ListItemActions(v, info));
            }

            v.ContentSearch.UpdateButtons();
        }

        void ListItemActions(DebugView v, ItemInfo info)
        {
            v.SetContent_Search();

            v.ContentSearch.AddItem($"Toggle owned: {Item.IsOwned(info.Type)}", () => ToggleItemOwned(v, info));

            v.ContentSearch.UpdateButtons();
        }

        void ToggleItemOwned(DebugView v, ItemInfo info)
        {
            Item.SetOwned(info.Type, !Item.IsOwned(info.Type));
            ListItemActions(v, info);
        }

        Debug.RegisterAction(new DebugAction
        {
            Category = category,
            Text = "Unlock everything",
            Action = UnlockEverything
        });

        Debug.RegisterAction(new DebugAction
        {
            Category = category,
            Text = "Lock everything",
            Action = LockEverything
        });

        void UnlockEverything(DebugView v)
        {
            var types = System.Enum.GetValues(typeof(ItemType)).Cast<ItemType>();

            foreach (var type in types)
            {
                Item.SetOwned(type, true);
            }

            v.Close();
        }

        void LockEverything(DebugView v)
        {
            var types = System.Enum.GetValues(typeof(ItemType)).Cast<ItemType>();

            foreach (var type in types)
            {
                if (Item.IsNoneType(type))
                {
                    Item.SetOwned(type, true);
                }
                else
                {
                    Item.SetOwned(type, false);
                }
            }

            v.Close();
        }
    }

    public ItemInfo GetInfo(ItemType type)
    {
        return Collection.Resources.FirstOrDefault(x => x.Type == type);
    }

    public IEnumerable<ItemInfo> GetInfos(ItemCategory category)
    {
        return Collection.Resources.Where(x => x.Category == category);
    }

    private void GameProfileSelected(int i)
    {
        var items = new List<ItemType>()
        {
            ItemType.BodyTop_None,
            ItemType.BodyPattern_None,
            ItemType.BodyEye_None,
            ItemType.Face_None,
            ItemType.Hat_None,
        };

        foreach (var type in items)
        {
            var item = Item.GetOrCreateData(type);
            item.Owned = true;
        }
    }
}
