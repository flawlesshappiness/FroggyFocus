using System.Linq;

public partial class FetchController : ResourceController<FetchCollection, FetchInfo>
{
    public static FetchController Instance => Singleton.Get<FetchController>();
    public override string Directory => "FetchQuest";

    public override void _Ready()
    {
        base._Ready();
        RegisterDebugActions();
    }

    private void RegisterDebugActions()
    {
        var category = "FETCH";

        Debug.RegisterAction(new DebugAction
        {
            Category = category,
            Text = "Reset all",
            Action = ResetAll
        });

        void ResetAll(DebugView v)
        {
            foreach (var data in Data.Game.Fetchs)
            {
                var info = GetInfo(data.Id);
                Fetch.ResetData(info);
                data.DateTimeNext = GameTime.GetCurrentDateTimeString();
            }

            Data.Game.Save();
            v.Close();
        }
    }

    public FetchInfo GetInfo(string id)
    {
        return Collection.Resources.FirstOrDefault(x => x.Id == id);
    }
}
