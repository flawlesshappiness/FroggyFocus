using Godot;
using System.Linq;

public partial class UpgradeController : ResourceController<UpgradeCollection, UpgradeInfo>
{
    public static UpgradeController Instance => Singleton.Get<UpgradeController>();
    public override string Directory => "Upgrade";

    private int[] DefaultPrices = [100, 500, 1000, 4000, 10000, 18000, 30000, 50000];

    public override void _Ready()
    {
        base._Ready();
        RegisterDebugActions();
    }

    private void RegisterDebugActions()
    {
        var category = "UPGRADES";

        Debug.RegisterAction(new DebugAction
        {
            Category = category,
            Text = "Set level",
            Action = v => SetLevel(v)
        });

        void SetLevel(DebugView v)
        {
            v.SetContent_Search();

            var infos = Collection.Resources;
            foreach (var info in infos)
            {
                v.ContentSearch.AddItem($"{info.Name} ({info.GetResourceName()})", () => SelectUpgradeLevel(v, info));
            }

            v.ContentSearch.UpdateButtons();
        }

        void SelectUpgradeLevel(DebugView v, UpgradeInfo info)
        {
            v.SetContent_Search();

            var data = GetOrCreateData(info.Type);
            for (int i = 0; i < info.Values.Count; i++)
            {
                var level = i;
                var selected = data.Level == i ? "> " : string.Empty;
                v.ContentSearch.AddItem($"{selected}{i}: {info.Values[i]}", () => SetUpgradeLevel(v, info, level));
            }

            v.ContentSearch.UpdateButtons();
        }

        void SetUpgradeLevel(DebugView v, UpgradeInfo info, int level)
        {
            var data = GetOrCreateData(info.Type);
            data.Level = level;
            Data.Game.Save();
            SetLevel(v);
        }
    }

    public UpgradeInfo GetInfo(UpgradeType type)
    {
        return Collection.Resources.FirstOrDefault(x => x.Type == type);
    }

    public UpgradeData GetOrCreateData(UpgradeType type)
    {
        var data = Data.Game.Upgrades.FirstOrDefault(x => x.Type == type);

        if (data == null)
        {
            data = new UpgradeData { Type = type };
            Data.Game.Upgrades.Add(data);
        }

        return data;
    }

    public int GetMaxLevel(UpgradeType type)
    {
        return GetInfo(type).Values.Count - 1;
    }

    public bool IsMaxLevel(UpgradeType type)
    {
        return GetCurrentLevel(type) >= GetMaxLevel(type);
    }

    public int GetCurrentLevel(UpgradeType type)
    {
        var data = GetOrCreateData(type);
        return data?.Level ?? 0;
    }

    public float GetCurrentValue(UpgradeType type)
    {
        var data = GetOrCreateData(type);
        return GetValue(type, data.Level);
    }

    public float GetValue(UpgradeType type, int level)
    {
        var info = GetInfo(type);
        var idx = Mathf.Clamp(level, 0, info.Values.Count - 1);
        return info.Values[idx];
    }

    public int GetCurrentPrice(UpgradeType type)
    {
        var level = GetCurrentLevel(type);
        return GetPrice(type, level);
    }

    public int GetPrice(UpgradeType type, int level)
    {
        return GetOverridePrice(type, level) ?? GetDefaultPrice(level);
    }

    public int GetDefaultPrice(int level)
    {
        if (DefaultPrices.Length <= level) return 100000;
        return DefaultPrices[level];
    }

    public int? GetOverridePrice(UpgradeType type, int level)
    {
        var info = GetInfo(type);
        if (info.OverridePrice == null) return null;
        if (info.OverridePrice.Count <= level) return null;

        return info.OverridePrice[level];
    }

    public bool TryPurchaseUpgrade(UpgradeType type)
    {
        var data = GetOrCreateData(type);
        var price = GetCurrentPrice(type);

        if (!Money.CanAfford(price)) return false;

        CurrencyController.Instance.AddValue(CurrencyType.Money, -price);
        data.Level++;

        Data.Game.Save();

        return true;
    }
}
