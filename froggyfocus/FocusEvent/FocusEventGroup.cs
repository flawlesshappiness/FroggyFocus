using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class FocusEventGroup : Node3D
{
    [Export]
    public int ActiveCount;

    private List<FocusEvent> FocusEvents { get; set; } = new();
    private List<FocusEvent> ActiveEvents { get; set; } = new();
    private List<FocusEvent> InactiveEvents { get; set; } = new();

    public override void _Ready()
    {
        base._Ready();
        InitializeEvents();

        for (int i = 0; i < ActiveCount; i++)
        {
            EnableInactiveEvent(true);
        }
    }

    private void InitializeEvents()
    {
        FocusEvents = this.GetNodesInChildren<FocusEvent>();
        InactiveEvents = FocusEvents.ToList();

        foreach (var e in FocusEvents)
        {
            e.OnEnabled += () => EventEnabled(e);
            e.OnDisabled += () => EventDisabled(e);
            e.OnStarted += () => EventStarted(e);
            e.OnCompleted += EventCompleted;
            e.OnFailed += EventFailed;
        }
    }

    private void EventStarted(FocusEvent e)
    {
        FocusEventController.Instance.FocusEventStarted(e);
    }

    private void EventEnabled(FocusEvent e)
    {

    }

    private void EventDisabled(FocusEvent e)
    {
        EnableInactiveEvent();
        SetEventInactive(e);
    }

    private void EventCompleted(FocusEventCompletedResult result)
    {
        FocusEventController.Instance.FocusEventCompleted(result);
    }

    private void EventFailed(FocusEventFailedResult result)
    {
        FocusEventController.Instance.FocusEventFailed(result);
    }

    private void EnableInactiveEvent(bool immediate = false)
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