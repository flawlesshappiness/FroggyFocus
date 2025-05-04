using System.Collections.Generic;
using System.Linq;

public partial class GameFlagsController : SingletonController
{
    public override string Directory => $"{Paths.Modules}/GameFlags";
    public static GameFlagsController Instance => Singleton.Get<GameFlagsController>();

    private Dictionary<string, GameFlagData> _flags = new();

    protected override void Initialize()
    {
        base.Initialize();
        GameProfileController.Instance.OnGameProfileSelected += GameProfileSelected;
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
    }
}
