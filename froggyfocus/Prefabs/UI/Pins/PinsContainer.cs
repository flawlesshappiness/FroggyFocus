using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class PinsContainer : ControlScript
{
    [Export]
    public PackedScene PinControlPrefab;

    [Export]
    public Control PinsParent;

    [Export]
    public ScrollContainer ScrollContainer;

    public event Action OnPinsEmpty;

    private bool self_changed;
    private List<PinnedHandInControl> pins = new();

    public override void _Ready()
    {
        base._Ready();
        HandInController.Instance.OnHandInPinned += HandInPinned;
        HandInController.Instance.OnHandInUnpinned += HandInUnpinned;
    }

    protected override void Initialize()
    {
        base.Initialize();
        GameProfileController.Instance.OnGameProfileSelected += GameProfileSelected;
        UpdateControls();
    }

    private void GameProfileSelected(int profile)
    {
        UpdateControls();
    }

    private void HandInPinned(string id)
    {
        UpdateControls();
    }

    private void HandInUnpinned(string id)
    {
        if (self_changed)
        {
            self_changed = false;
            return;
        }

        UpdateControls();
    }

    private void Unpin_Pressed(PinnedHandInControl pin)
    {
        self_changed = true;
        pin.QueueFree();
        pins.Remove(pin);
        UpdateFocus();

        HandInController.Instance.UnpinHandIn(pin.HandInData.Id);
    }

    private void Clear()
    {
        foreach (var pin in pins)
        {
            pin.QueueFree();
        }

        pins.Clear();
    }

    private void UpdateControls()
    {
        Clear();

        foreach (var data in Data.Game.HandIns)
        {
            if (!data.Pinned) continue;
            var pin = CreatePin();
            pin.Initialize(data);
        }

        var first = pins.FirstOrDefault();
        if (first != null)
        {
            first.AnyFocusEntered += ScrollContainer.ScrollVerticalToTop;
        }

        var last = pins.LastOrDefault();
        if (last != null)
        {
            last.AnyFocusEntered += ScrollContainer.ScrollVerticalToBottom;
        }
    }

    private PinnedHandInControl CreatePin()
    {
        var pin = PinControlPrefab.Instantiate<PinnedHandInControl>();
        pin.SetParent(PinsParent);
        pins.Add(pin);
        pin.UnpinButton.Pressed += () => Unpin_Pressed(pin);
        return pin;
    }

    private void UpdateFocus()
    {
        var control = GetFocusControl();
        if (control == null)
        {
            OnPinsEmpty?.Invoke();
        }
        else
        {
            control.GrabFocus();
        }
    }

    public Control GetFocusControl()
    {
        var control = pins.FirstOrDefault();
        return control?.UnpinButton;
    }

    private void ScrollToTop()
    {
        ScrollContainer.ScrollVertical = 0;
    }

    private void ScrollToBottom()
    {
        ScrollContainer.ScrollVertical = int.MaxValue;
    }
}
