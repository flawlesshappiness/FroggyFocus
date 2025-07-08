using System.Linq;

public partial class StatsController : SingletonController
{
    public override string Directory => "Stats";
    public static StatsController Instance => Singleton.Get<StatsController>();

    protected override void Initialize()
    {
        base.Initialize();
        FocusEventController.Instance.OnFocusEventCompleted += FocusEventCompleted;
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

            foreach (var data in GetData().Characters)
            {
                var name = System.IO.Path.GetFileName(data.InfoPath).RemoveExtension();
                v.ContentSearch.AddItem(name, () => ShowCharacter(v, data));
            }

            v.ContentSearch.UpdateButtons();
        }

        void ShowCharacter(DebugView v, StatsCharacterData data)
        {
            v.HideContent();
            v.SetContent_List();
            v.ContentList.AddText($"Caught: {data.CountCaught}");
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

    private void FocusEventCompleted(FocusEventCompletedResult result)
    {
        var info = result.FocusEvent.Target.Info;
        var data = GetOrCreateCharacterData(info.ResourcePath);
        data.CountCaught++;
    }
}
