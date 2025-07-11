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
                v.ContentSearch.AddItem(text, () => SelectLevel(v, info));
            }

            v.ContentSearch.UpdateButtons();
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

            v.ContentSearch.UpdateButtons();
        }

        void SetLevel(DebugView v, ObjectiveInfo info, int level)
        {
            Objective.SetLevel(info, level);
            Data.Game.Save();
            SelectLevel(v, info);
        }
    }

    public ObjectiveInfo GetInfoFromPath(string path)
    {
        return Collection.Resources.FirstOrDefault(x => x.ResourcePath == path);
    }

    private void FocusEventCompleted(FocusEventCompletedResult result)
    {
        var info = result.FocusEvent.Target.Info;

        foreach (var objective in Collection.Resources)
        {
            if (info.Tags.Any(x => x == objective.RequirementTag))
            {
                var data = Objective.GetOrCreateData(objective.ResourcePath);
                data.Value++;
            }
        }

        Data.Game.Save();
    }
}
