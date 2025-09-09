using System;
using System.Linq;

public partial class HandInController : ResourceController<HandInCollection, HandInInfo>
{
    public override string Directory => "HandInQuest";
    public static HandInController Instance => Singleton.Get<HandInController>();

    public event Action<string> OnHandInClaimed;
    public event Action<string> OnHandInClosed;

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
            Text = "Edit",
            Action = SelectHandIn
        });

        void SelectHandIn(DebugView v)
        {
            v.SetContent_Search();

            foreach (var info in Collection.Resources)
            {
                v.ContentSearch.AddItem(info.Id, () => HandInActions(v, info));
            }

            v.ContentSearch.UpdateButtons();
        }

        void HandInActions(DebugView v, HandInInfo info)
        {
            v.SetContent_Search();

            v.ContentSearch.AddItem("Show", () => ShowHandIn(v, info));
            v.ContentSearch.AddItem("Set claimed count", () => SelectClaimedCount(v, info));
            v.ContentSearch.AddItem("Make available", () => MakeAvailable(v, info));
            v.ContentSearch.AddItem("Make available in 10 seconds", () => MakeAvailableSoon(v, info));
            v.ContentSearch.AddItem("Reset", () => Reset(v, info));

            v.ContentSearch.UpdateButtons();
        }

        void ShowHandIn(DebugView v, HandInInfo info)
        {
            HandInView.Instance.ShowPopup(info.Id);
            v.Close();
        }

        void MakeAvailable(DebugView v, HandInInfo info)
        {
            var data = HandIn.GetOrCreateData(info.Id);
            data.DateTimeNext = GameTime.GetCurrentDateTimeString();
            Data.Game.Save();

            HandInActions(v, info);
        }

        void MakeAvailableSoon(DebugView v, HandInInfo info)
        {
            var data = HandIn.GetOrCreateData(info.Id);
            var date = GameTime.GetCurrentDateTime().AddSeconds(10);
            data.DateTimeNext = GameTime.GetDateTimeString(date);
            Data.Game.Save();

            ShowHandIn(v, info);
        }

        void SelectClaimedCount(DebugView v, HandInInfo info)
        {
            v.SetContent_Search();

            for (int i = 0; i < info.ClaimCountToUnlock + 1; i++)
            {
                var idx = i;
                v.ContentSearch.AddItem($"{idx}", () => SetClaimedCount(v, info, idx));
            }

            v.ContentSearch.UpdateButtons();
        }

        void SetClaimedCount(DebugView v, HandInInfo info, int i)
        {
            var data = HandIn.GetOrCreateData(info.Id);
            data.ClaimedCount = i;
            Data.Game.Save();

            HandInActions(v, info);
        }

        void Reset(DebugView v, HandInInfo info)
        {
            HandIn.ResetData(info);
            Data.Game.Save();

            HandInActions(v, info);
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

    public void HandInClosed(HandInInfo info)
    {
        OnHandInClosed?.Invoke(info.Id);
    }
}
