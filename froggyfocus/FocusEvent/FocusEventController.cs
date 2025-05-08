using System;
using System.Collections.Generic;
using System.Linq;

public partial class FocusEventController : ResourceController<FocusEventCollection, FocusEventInfo>
{
    public override string Directory => "FocusEvent";
    public static FocusEventController Instance => Singleton.Get<FocusEventController>();

    public event Action OnFocusEventEnabled;
    public event Action OnFocusEventDisabled;
    public event Action OnFocusEventStarted;
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

    public void EnableInactiveEvent(bool immediate = false)
    {
        var e = InactiveEvents.Random();
        SetEventActive(e);

        if (immediate)
        {
            e.EnableEvent();
        }
        else
        {
            e.EnableEventAfterDelay();
        }
    }

    public void FocusEventStarted(FocusEvent e)
    {
        OnFocusEventStarted?.Invoke();
    }

    public void FocusEventCompleted(FocusEvent e)
    {
        OnFocusEventCompleted?.Invoke();
    }

    public void FocusEventFailed(FocusEvent e)
    {
        OnFocusEventFailed?.Invoke();
    }

    public void EventDisabled(FocusEvent e)
    {
        SetEventInactive(e);
        OnFocusEventDisabled?.Invoke();
    }

    public void EventEnabled(FocusEvent e)
    {
        SetEventActive(e);
        OnFocusEventEnabled?.Invoke();
    }

    private void SetEventActive(FocusEvent e)
    {
        if (!ActiveEvents.Contains(e)) ActiveEvents.Add(e);
        if (InactiveEvents.Contains(e)) InactiveEvents.Remove(e);
    }

    private void SetEventInactive(FocusEvent e)
    {
        if (ActiveEvents.Contains(e)) ActiveEvents.Remove(e);
        if (!InactiveEvents.Contains(e)) InactiveEvents.Add(e);
    }
}
