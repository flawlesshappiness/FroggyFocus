using System;
using System.Linq;

public partial class HandInController : ResourceController<HandInCollection, HandInInfo>
{
    public override string Directory => "HandInQuest";
    public static HandInController Instance => Singleton.Get<HandInController>();

    public event Action<string> OnHandInClaimed;

    public override void _Ready()
    {
        base._Ready();
        RegisterDebugActions();
    }

    private void RegisterDebugActions()
    {
        var category = "HAND IN";

        Debug.RegisterAction(new DebugAction
        {
            Category = category,
            Text = "Reset all",
            Action = ResetAll
        });

        void ResetAll(DebugView v)
        {
            v.Close();

            foreach (var data in Data.Game.HandIns)
            {
                var info = GetInfo(data.Id);
                HandIn.ResetData(info);
                data.DateTimeNext = GameTime.GetCurrentDateTimeString();
            }

            Data.Game.Save();
        }
    }

    public HandInInfo GetInfo(string id)
    {
        return Collection.Resources.FirstOrDefault(x => x.Id == id);
    }

    public void HandInClaimed(HandInData data)
    {
        OnHandInClaimed?.Invoke(data.Id);
    }
}
