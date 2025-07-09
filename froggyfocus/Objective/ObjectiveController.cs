using System.Linq;

public partial class ObjectiveController : ResourceController<ObjectiveCollection, ObjectiveInfo>
{
    public static ObjectiveController Instance => Singleton.Get<ObjectiveController>();
    public override string Directory => "Objective";

    protected override void Initialize()
    {
        base.Initialize();
        FocusEventController.Instance.OnFocusEventCompleted += FocusEventCompleted;
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
