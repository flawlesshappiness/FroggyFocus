using System;
using System.Collections.Generic;
using System.Linq;

public partial class GameFlagsController : SingletonController
{
    public override string Directory => $"{Paths.Modules}/GameFlags";
    public static GameFlagsController Instance => Singleton.Get<GameFlagsController>();

    public event Action<string, int> OnFlagChanged;

    private Dictionary<string, GameFlagData> _flags = new();

    protected override void Initialize()
    {
        base.Initialize();
        GameProfileController.Instance.OnGameProfileSelected += GameProfileSelected;
        RegisterDebugActions();
    }

    private void RegisterDebugActions()
    {
        var category = "GAME FLAGS";

        Debug.RegisterAction(new DebugAction
        {
            Category = category,
            Text = "Select flag",
            Action = ListFlags
        });

        void ListFlags(DebugView v)
        {
            v.SetContent_Search();

            foreach (var flag in _flags)
            {
                v.ContentSearch.AddItem(flag.Key, () => AdjustFlag(v, flag.Value));
            }

            v.ContentSearch.UpdateButtons();
        }

        void AdjustFlag(DebugView v, GameFlagData data)
        {
            v.SetContent_Search();

            v.ContentSearch.AddItem($"Value = {data.Value}", () => { });
            v.ContentSearch.AddItem("0", () => DebugSetFlag(v, data, 0));
            v.ContentSearch.AddItem("1", () => DebugSetFlag(v, data, 1));
            v.ContentSearch.AddItem("+", () => DebugSetFlag(v, data, data.Value + 1));
            v.ContentSearch.AddItem("-", () => DebugSetFlag(v, data, data.Value - 1));
            v.ContentSearch.AddItem("Back", () => ListFlags(v));

            v.ContentSearch.UpdateButtons();
        }

        void DebugSetFlag(DebugView v, GameFlagData data, int value)
        {
            SetFlag(data.Id, value);
            AdjustFlag(v, data);
        }
    }

    private void GameProfileSelected(int i)
    {
        Load();
    }

    public void Load()
    {
        _flags.Clear();
        foreach (var data in Data.Game.GameFlags)
        {
            _flags.Add(data.Id, data);
        }
    }

    private void UpdateData()
    {
        Data.Game.GameFlags = _flags.Values.ToList();
    }

    public GameFlagData GetOrCreateFlag(string id)
    {
        if (!_flags.ContainsKey(id))
        {
            _flags.Add(id, new GameFlagData { Id = id });
        }

        return _flags[id];
    }

    public bool HasFlag(string id)
    {
        return !IsFlag(id, 0);
    }

    public bool IsFlag(string id, int value)
    {
        return GetOrCreateFlag(id).Value == value;
    }

    public void SetFlag(string id, int value)
    {
        GetOrCreateFlag(id).Value = value;
        UpdateData();

        OnFlagChanged?.Invoke(id, value);
    }
}
