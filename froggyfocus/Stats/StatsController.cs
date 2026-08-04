using Godot;
using System.Linq;

public partial class StatsController : SingletonController
{
    public override string Directory => "Stats";
    public static StatsController Instance => Singleton.Get<StatsController>();

    protected override void Initialize()
    {
        base.Initialize();
        InventoryController.Instance.OnCharacterAdded += InventoryCharacter_Added;
        FocusEventController.Instance.OnFocusEventEnded += FocusEvent_Ended;
        RegisterDebugActions();
    }

    private void RegisterDebugActions()
    {
        var category = "STATS";

        Debug.RegisterAction(new DebugAction
        {
            Category = category,
            Text = "Show stats",
            Action = ShowStats
        });

        void ShowStats(DebugView v)
        {
            v.HideContent();
            v.SetContent_Search();
            v.ContentSearch.AddItem("Characters", () => ShowCharacters(v));
            v.ContentSearch.UpdateButtons();
        }

        void ShowCharacters(DebugView v)
        {
            v.HideContent();
            v.SetContent_Search();

            var infos = FocusCharacterController.Instance.Collection.Resources;
            foreach (var info in infos)
            {
                var data = GetOrCreateCharacterData(info.ResourcePath);
                var name = System.IO.Path.GetFileName(data.InfoPath).RemoveExtension();
                v.ContentSearch.AddItem(name, () => SelectCharacter(v, data));
            }

            v.ContentSearch.UpdateButtons();
        }

        void SelectCharacter(DebugView v, StatsCharacterData data)
        {
            v.HideContent();
            v.SetContent_Search();
            v.ContentSearch.AddItem($"Caught: {data.CountCaught}", () => EditCaught(v, data));
            v.ContentSearch.UpdateButtons();
        }

        void EditCaught(DebugView v, StatsCharacterData data)
        {
            v.HideContent();
            v.SetContent_Search();
            v.ContentSearch.AddItem($"0", () => SetCaught(v, data, 0));
            v.ContentSearch.AddItem($"1", () => SetCaught(v, data, 1));
            v.ContentSearch.UpdateButtons();
        }

        void SetCaught(DebugView v, StatsCharacterData data, int amount)
        {
            data.CountCaught = amount;
            Data.Game.Save();
            SelectCharacter(v, data);
        }

        Debug.RegisterAction(new DebugAction
        {
            Category = category,
            Text = "Set all caught",
            Action = SetAllCaught
        });

        void SetAllCaught(DebugView v)
        {
            var infos = FocusCharacterController.Instance.Collection.Resources;
            var datas = infos.Select(x => GetOrCreateCharacterData(x.ResourcePath));
            datas.ForEach(x => x.CountCaught = 1);
            Data.Game.Save();
        }

        Debug.RegisterAction(new DebugAction
        {
            Category = category,
            Text = "Set all uncaught",
            Action = SetAllUncaught
        });

        void SetAllUncaught(DebugView v)
        {
            var infos = FocusCharacterController.Instance.Collection.Resources;
            var datas = infos.Select(x => GetOrCreateCharacterData(x.ResourcePath));
            datas.ForEach(x => x.CountCaught = 0);
            Data.Game.Save();
        }
    }

    private StatsData GetData()
    {
        Data.Game.Stats ??= new();
        return Data.Game.Stats;
    }

    public StatsCharacterData GetOrCreateCharacterData(string info_path)
    {
        var data = GetData();
        var character = data.Characters.FirstOrDefault(x => x.InfoPath == info_path);

        if (character == null)
        {
            character = new StatsCharacterData { InfoPath = info_path };
            data.Characters.Add(character);
        }

        return character;
    }

    private void InventoryCharacter_Added(InventoryCharacterData data)
    {
        var info = FocusCharacterController.Instance.GetInfoFromPath(data.InfoPath);
        var stats = GetOrCreateCharacterData(info.ResourcePath);
        stats.CountCaught++;
        stats.HighestRarity = Mathf.Max(stats.HighestRarity, data.Stars);

        var v_info = FocusCharacterController.Instance.Collection.Resources.FirstOrDefault(x => x.Name == info.Variation);
        if (v_info == info) return;

        var v_stats = GetOrCreateCharacterData(v_info.ResourcePath);
        v_stats.CountCaught++;
        v_stats.HighestRarity = Mathf.Max(stats.HighestRarity, data.Stars);
    }

    public bool HasCaughtAllBugs()
    {
        var infos = FocusCharacterController.Instance.Collection.Resources.Where(x => !x.ExcludeFromBestiary);
        var stats = infos.Select(x => GetOrCreateCharacterData(x.ResourcePath));
        var all_caught = stats.All(x => x.CountCaught > 0);
        return all_caught;
    }

    private void FocusEvent_Ended(FocusEventResult result)
    {
        UnlockCrown();
    }

    private void UnlockCrown()
    {
        if (Item.IsOwned(ItemType.Hat_Crown)) return;
        if (!HasCaughtAllBugs()) return;

        Item.MakeOwned(ItemType.Hat_Crown);
        UnlockView.Instance.ShowItemUnlock(ItemType.Hat_Crown);
    }
}
