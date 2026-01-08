using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class OptionsKeys : NodeScript
{
    [Export]
    public OptionsKeyRebindControl TempKeyRebindControl;

    [Export]
    public Button ResetAllButton;

    public bool IsRebinding => current_control != null;

    public event Action OnRebindStart, OnRebindEnd;

    private OptionsKeyRebindControl current_control;
    private Dictionary<string, OptionsKeyRebindControl> rebind_controls = new();

    public override void _Ready()
    {
        base._Ready();
        ResetAllButton.Pressed += PressResetAllRebinds;
    }

    protected override void Initialize()
    {
        base.Initialize();
        CreateKeys();
    }

    private void RebindStarted()
    {
        SetButtonsEnabled(false);
        OnRebindStart?.Invoke();
    }

    private void RebindEnded()
    {
        SetButtonsEnabled(true);
        OnRebindEnd?.Invoke();
    }

    private void CreateKeys()
    {
        TempKeyRebindControl.Visible = false;

        var parent = TempKeyRebindControl.GetParent();
        foreach (var action in OptionsKeysInfo.Instance.Actions)
        {
            var control = TempKeyRebindControl.Duplicate() as OptionsKeyRebindControl;
            control.SetParent(parent);
            control.Show();
            control.RebindLabel.Text = action;
            control.SetWaitingForInput(false);
            rebind_controls.Add(action, control);

            control.Action = action;
            control.Rebind = OptionsController.Rebinds.First(x => x.Action == action);

            control.RebindButton.Pressed += () => PressRebind(control);
            control.ResetButton.Pressed += () => PressResetRebind(control);
        }
    }

    public void UpdateAllKeyStrings()
    {
        rebind_controls.Values.ForEach(UpdateKeyString);
    }

    private void UpdateKeyString(OptionsKeyRebindControl control)
    {
        var e = InputMap.ActionGetEvents(control.Action).First(x => x is InputEventKey || x is InputEventMouseButton);
        var text = e.AsText().Replace("(Physical)", "").Trim();
        control.RebindButton.Text = text;
    }

    public void UpdateDuplicateWarnings()
    {
        foreach (var current in OptionsController.Rebinds)
        {
            var current_input = InputMap.ActionGetEvents(current.Action).FirstOrDefault();
            if (current_input == null) continue;

            var found_duplicate = false;

            foreach (var other in OptionsController.Rebinds)
            {
                if (current == other) continue;

                var other_input = InputMap.ActionGetEvents(other.Action).FirstOrDefault();
                if (other_input == null) continue;

                var same = current_input.AsText() == other_input.AsText();
                found_duplicate = same || found_duplicate;

                if (rebind_controls.TryGetValue(current.Action, out var control))
                {
                    control.DuplicateWarningLabel.Visible = found_duplicate;
                }

                if (found_duplicate) break;
            }
        }
    }

    private void PressRebind(OptionsKeyRebindControl control)
    {
        if (current_control != null) return;

        current_control = control;
        control.SetWaitingForInput(true);
        RebindStarted();
    }

    private void PressResetAllRebinds()
    {
        ResetAllRebinds();
        UpdateDuplicateWarnings();
        Data.Game.Save();
    }

    private void PressResetRebind(OptionsKeyRebindControl control)
    {
        ResetRebind(control);
        UpdateDuplicateWarnings();
        Data.Game.Save();
    }

    private void ResetAllRebinds()
    {
        rebind_controls.Values.ForEach(ResetRebind);
    }

    private void ResetRebind(OptionsKeyRebindControl control)
    {
        var action = control.Action;
        var bindings = OptionsController.DefaultBindings[action];
        InputMap.ActionEraseEvents(action);

        foreach (var binding in bindings)
        {
            InputMap.ActionAddEvent(action, binding);
        }
        control.Rebind.Data = null;

        UpdateKeyString(control);
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (current_control == null) return;

        if (IsCancelEvent(@event))
        {
            StopRebinding();
        }
        else if (@event is InputEventKey)
        {
            OverrideKey(@event as InputEventKey);
            StopRebinding();
        }
        else if (@event is InputEventMouseButton)
        {
            OverrideMouseButton(@event as InputEventMouseButton);
            StopRebinding();
        }
    }

    private void OverrideKey(InputEventKey e)
    {
        var data = InputEventKeyData.Create(current_control.Action, e);
        current_control.Rebind.Data = data;
        OptionsController.Instance.UpdateKeyOverride(data);
    }

    private void OverrideMouseButton(InputEventMouseButton e)
    {
        var data = InputEventMouseButtonData.Create(current_control.Action, e);
        current_control.Rebind.Data = data;
        OptionsController.Instance.UpdateMouseButtonOverride(data);
    }

    private bool IsCancelEvent(InputEvent e)
    {
        var key_event = e as InputEventKey;
        if (key_event == null) return false;
        return key_event.KeyLabel == Key.Escape;
    }

    private void StopRebinding()
    {
        current_control.SetWaitingForInput(false);
        UpdateAllKeyStrings();
        UpdateDuplicateWarnings();
        RebindEnded();

        current_control.RebindButton.GrabFocus();
        current_control = null;
    }

    private void SetButtonsEnabled(bool enabled)
    {
        foreach (var control in rebind_controls.Values)
        {
            control.RebindButton.Disabled = !enabled;
            control.ResetButton.Disabled = !enabled;
        }

        ResetAllButton.Disabled = !enabled;
    }
}
