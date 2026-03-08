using System;
using System.Linq;

public partial class HandInController : ResourceController<HandInCollection, HandInInfo>
{
    public override string Directory => "HandIn";
    public static HandInController Instance => Singleton.Get<HandInController>();

    public event Action<string> OnHandInClaimed;
    public event Action<string> OnHandInClosed;
    public event Action<string> OnHandInPinned;
    public event Action<string> OnHandInUnpinned;

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

            v.ContentSearch.UpdateButtons();
        }

        void ShowHandIn(DebugView v, HandInInfo info)
        {
            HandInView.Instance.ShowPopup(info.Id);
            v.Close();
        }

        void SelectClaimedCount(DebugView v, HandInInfo info)
        {
            v.SetContent_Search();

            var data = HandIn.GetOrCreateData(info.Id);
            v.ContentSearch.AddItem($"Current: {data.ClaimCount}", () => { });
            v.ContentSearch.AddItem($"+1", () => SetClaimedCount(v, info, data, data.ClaimCount + 1));
            v.ContentSearch.AddItem($"-1", () => SetClaimedCount(v, info, data, data.ClaimCount - 1));
            v.ContentSearch.AddItem($"Back", () => HandInActions(v, info));

            v.ContentSearch.UpdateButtons();
        }

        void SetClaimedCount(DebugView v, HandInInfo info, HandInData data, int i)
        {
            data.ClaimCount = i;
            Data.Game.Save();

            SelectClaimedCount(v, info);
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

    public void PinHandIn(string id)
    {
        var data = HandIn.GetOrCreateData(id);
        data.Pinned = true;
        Data.Game.Save();

        OnHandInPinned?.Invoke(id);
    }

    public void UnpinHandIn(string id)
    {
        var data = HandIn.GetOrCreateData(id);
        data.Pinned = false;
        Data.Game.Save();

        OnHandInUnpinned?.Invoke(id);
    }
}
