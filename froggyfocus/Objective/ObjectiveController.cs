using System.Linq;

public partial class ObjectiveController : ResourceController<ObjectiveCollection, ObjectiveInfo>
{
    public static ObjectiveController Instance => Singleton.Get<ObjectiveController>();
    public override string Directory => "Objective";

    protected override void Initialize()
    {
        base.Initialize();
        FocusEventController.Instance.OnFocusEventCompleted += FocusEventCompleted;
        RegisterDebugActions();
    }

    private void RegisterDebugActions()
    {
        var category = "OBJECTIVE";

        Debug.RegisterAction(new DebugAction
        {
            Category = category,
            Text = "Edit objectives",
            Action = SelectObjective
        });

        void SelectObjective(DebugView v)
        {
            v.SetContent_Search();

            foreach (var info in Collection.Resources)
            {
                var text = $"{info.Description} ({info.GetResourceName()})";
                v.ContentSearch.AddItem(text, () => SelectAction(v, info));
            }

            v.ContentSearch.UpdateButtons();
        }

        void SelectAction(DebugView v, ObjectiveInfo info)
        {
            v.SetContent_Search();

            v.ContentSearch.AddItem("Info", () => ShowInfo(v, info));
            v.ContentSearch.AddItem("Level", () => SelectLevel(v, info));
            v.ContentSearch.AddItem("Value", () => SelectValue(v, info));

            v.ContentSearch.UpdateButtons();
        }

        void ShowInfo(DebugView v, ObjectiveInfo info)
        {
            v.SetContent_List();

            var data = Objective.GetOrCreateData(info.ResourcePath);

            v.ContentList.AddText($"Resource: {info.GetResourceName()}");
            v.ContentList.AddText($"Description: {info.Description}");
            v.ContentList.AddText($"Tag: {info.RequirementTag}");
            v.ContentList.AddText($"Level: {data.Level}");
            v.ContentList.AddText($"Value: {data.Value} / {info.Values[data.Level]}");
            v.ContentList.AddText($"Reward: {info.MoneyRewards[data.Level]}");
        }

        void SelectLevel(DebugView v, ObjectiveInfo info)
        {
            v.SetContent_Search();
            var data = Objective.GetOrCreateData(info.ResourcePath);

            for (int i = 0; i < info.Values.Count; i++)
            {
                var selected = i == data.Level ? "> " : string.Empty;
                var text = $"{selected}{info.Values[i]}";
                var level = i;
                v.ContentSearch.AddItem(text, () => SetLevel(v, info, level));
            }

            v.ContentSearch.AddItem("Back", () => SelectAction(v, info));
            v.ContentSearch.UpdateButtons();
        }

        void SetLevel(DebugView v, ObjectiveInfo info, int level)
        {
            Objective.SetLevel(info, level);
            Data.Game.Save();
            SelectLevel(v, info);
        }

        void SelectValue(DebugView v, ObjectiveInfo info)
        {
            v.SetContent_Search();

            v.ContentSearch.AddItem("Zero", () => SetValueZero(v, info));
            v.ContentSearch.AddItem("Max", () => SetValueMax(v, info));

            v.ContentSearch.UpdateButtons();
        }

        void SetValueZero(DebugView v, ObjectiveInfo info)
        {
            Objective.SetValue(info, 0);
            Data.Game.Save();
            SelectAction(v, info);
        }

        void SetValueMax(DebugView v, ObjectiveInfo info)
        {
            var data = Objective.GetOrCreateData(info.ResourcePath);
            Objective.SetValue(info, info.Values[data.Level]);
            Data.Game.Save();
            SelectAction(v, info);
        }
    }

    public ObjectiveInfo GetInfoFromPath(string path)
    {
        return Collection.Resources.FirstOrDefault(x => x.ResourcePath == path);
    }

    private void FocusEventCompleted(FocusEventCompletedResult result)
    {
        var target = result.FocusEvent.Target;
        var info = result.FocusEvent.Target.Info;

        foreach (var objective in Collection.Resources)
        {
            var valid_tag = !objective.UseTag || info.Tags.Any(x => x == objective.RequirementTag);
            var valid_rarity = target.CharacterData.Stars >= objective.MinimumStars;
            var valid = valid_tag && valid_rarity;
            if (valid)
            {
                Objective.AddValue(objective, 1);
            }
        }

        Data.Game.Save();
    }
}
