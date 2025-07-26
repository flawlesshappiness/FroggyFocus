using System;
using System.Linq;

public partial class FocusEventController : ResourceController<FocusEventCollection, FocusEventInfo>
{
    public override string Directory => "FocusEvent";
    public static FocusEventController Instance => Singleton.Get<FocusEventController>();

    public event Action OnFocusEventStarted;
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
                v.ContentSearch.AddItem(info.Name, () => StartFocusEvent(v, info));
            }

            v.ContentSearch.UpdateButtons();
        }

        void StartFocusEvent(DebugView v, FocusCharacterInfo info)
        {
            var focus_event = GameScene.Instance.FocusEvents
                .FirstOrDefault(x => x.Info.Characters.Contains(info))
                ?? GameScene.Instance.FocusEvents.FirstOrDefault();

            focus_event.DebugTargetInfo = info;
            focus_event.StartEvent();

            v.Close();
        }
    }

    public void FocusEventStarted(FocusEvent e)
    {
        OnFocusEventStarted?.Invoke();
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