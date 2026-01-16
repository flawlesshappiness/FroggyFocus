using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class DebugInputPopup : ControlScript
{
    [Export]
    public DebugInputString StringInput;

    [Export]
    public Button AcceptButton;

    [Export]
    public Button CancelButton;

    public event Action<Dictionary<string, string>> OnSuccess;
    public event Action OnCancel;

    private List<DebugInputString> _inputs = new();

    public override void _Ready()
    {
        base._Ready();
        StringInput.Hide();
        AcceptButton.Pressed += PressedAccept;
        CancelButton.Pressed += PressedCancel;
    }

    public void Clear()
    {
        foreach (var input in _inputs)
        {
            input.QueueFree();
        }

        _inputs.Clear();
    }

    public void CreateStringInput(string id, string label)
    {
        var input = StringInput.Duplicate() as DebugInputString;
        input.SetParent(StringInput.GetParent());

        input.Id = id;
        input.Label.Text = label;
        input.Show();

        _inputs.Add(input);
    }

    public void SetInputTexts(List<string> texts)
    {
        for (int i = 0; i < _inputs.Count; i++)
        {
            if (i >= texts.Count) break;
            var text = texts[i];
            var input = _inputs[i];
            input.Text.Text = text;
        }
    }

    public void InputGrabFocus()
    {
        _inputs.FirstOrDefault().Text.GrabFocus();
    }

    private Dictionary<string, string> GetInputResults()
    {
        var result = new Dictionary<string, string>();

        foreach (var input in _inputs)
        {
            result.Add(input.Id, input.Text.Text);
        }

        return result;
    }

    private void PressedAccept()
    {
        var result = GetInputResults();
        OnSuccess?.Invoke(result);
    }

    private void PressedCancel()
    {
        OnCancel?.Invoke();
    }
}
