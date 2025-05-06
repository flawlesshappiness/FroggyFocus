using System;
using System.Collections.Generic;
using System.Linq;

public partial class FocusEventController : ResourceController<FocusEventCollection, FocusEventInfo>
{
    public override string Directory => "FocusEvent";
    public static FocusEventController Instance => Singleton.Get<FocusEventController>();

    public event Action OnFocusEventEnabled;
    public event Action OnFocusEventDisabled;
    public event Action OnFocusEventCompleted;
    public event Action OnFocusEventFailed;

    private List<FocusEvent> FocusEvents { get; set; } = new();
    private List<FocusEvent> ActiveEvents { get; set; } = new();
    private List<FocusEvent> InactiveEvents { get; set; } = new();

    public void ClearEvents()
    {
        FocusEvents.Clear();
        ActiveEvents.Clear();
        InactiveEvents.Clear();
    }

    public void RegisterEvents(List<FocusEvent> events)
    {
        FocusEvents = events.ToList();
        InactiveEvents = events.ToList();
    }

    public void EnableInactiveEvent()
    {
        var e = InactiveEvents.Random();
        e.EnableEvent();
    }

    public void FocusEventCompleted(FocusEvent e)
    {
        OnFocusEventCompleted?.Invoke();
    }

    public void FocusEventFailed(FocusEvent e)
    {
        OnFocusEventFailed?.Invoke();
    }

    public void DisableEvent(FocusEvent e)
    {
        ActiveEvents.Remove(e);
        InactiveEvents.Add(e);
        OnFocusEventDisabled?.Invoke();
    }

    public void EnableEvent(FocusEvent e)
    {
        ActiveEvents.Add(e);
        InactiveEvents.Remove(e);
        OnFocusEventEnabled?.Invoke();
    }
}
