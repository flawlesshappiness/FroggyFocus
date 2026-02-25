using System;
using System.Linq;

public partial class FocusEventController : ResourceController<FocusEventCollection, FocusEventInfo>
{
    public override string Directory => "FocusEvent";
    public static FocusEventController Instance => Singleton.Get<FocusEventController>();

    public event Action<FocusEvent> OnFocusEventStarted;
    public event Action<FocusEventCompletedResult> OnFocusEventCompleted;
    public event Action<FocusEventFailedResult> OnFocusEventFailed;

    public override void _Ready()
    {
        base._Ready();
        RegisterDebugActions();
    }

    private void RegisterDebugActions()
    {
        var category = "FOCUS EVENT";

        Debug.RegisterAction(new DebugAction
        {
            Category = category,
            Text = "Start focus event",
            Action = SelectTarget
        });

        void SelectTarget(DebugView v)
        {
            v.SetContent_Search();

            var infos = FocusCharacterController.Instance.Collection.Resources;
            foreach (var info in infos)
            {
                v.ContentSearch.AddItem(info.Name, () => SelectStars(v, info));
            }

            v.ContentSearch.UpdateButtons();
        }

        void SelectStars(DebugView v, FocusCharacterInfo info)
        {
            v.SetContent_Search();

            for (int i = 1; i < 6; i++)
            {
                var stars = i;
                v.ContentSearch.AddItem($"{i} stars", () => StartFocusEvent(v, info, stars));
            }

            v.ContentSearch.UpdateButtons();
        }

        void StartFocusEvent(DebugView v, FocusCharacterInfo info, int stars)
        {
            var infos = Collection.Resources.First(x => x.Characters.Contains(info));
            var focus_event = GameScene.Instance.FocusEvent;

            focus_event.StartEvent(new FocusEvent.Settings
            {
                OverrideTargetInfo = info,
                OverrideTargetStars = stars
            });

            v.Close();
        }
    }

    public FocusEventInfo GetInfo(string id)
    {
        return Collection.GetResource(x => x.Id == id);
    }

    public void FocusEventStarted(FocusEvent e)
    {
        OnFocusEventStarted?.Invoke(e);
    }

    public void FocusEventCompleted(FocusEventCompletedResult result)
    {
        OnFocusEventCompleted?.Invoke(result);
    }

    public void FocusEventFailed(FocusEventFailedResult result)
    {
        OnFocusEventFailed?.Invoke(result);
    }
}

public class FocusEventResult
{
    public FocusEvent FocusEvent { get; set; }

    public FocusEventResult(FocusEvent e)
    {
        FocusEvent = e;
    }
}

public class FocusEventCompletedResult : FocusEventResult
{
    public FocusEventCompletedResult(FocusEvent e) : base(e) { }
}

public class FocusEventFailedResult : FocusEventResult
{
    public FocusEventFailedResult(FocusEvent e) : base(e) { }
}