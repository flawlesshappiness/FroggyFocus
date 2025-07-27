using Godot;
using Godot.Collections;
using System;

public partial class OptionsButtonControl : HBoxContainer
{
    [Export]
    public Array<string> Options;

    [Export]
    public Button PreviousButton;

    [Export]
    public Button NextButton;

    [Export]
    public Label Label;

    public event Action<int> IndexChanged;

    private int idx;

    public override void _Ready()
    {
        base._Ready();
        PreviousButton.Pressed += PreviousButton_Pressed;
        NextButton.Pressed += NextButton_Pressed;
    }

    private void PreviousButton_Pressed()
    {
        SetIndex(idx - 1);
        IndexChanged?.Invoke(idx);
    }

    private void NextButton_Pressed()
    {
        SetIndex(idx + 1);
        IndexChanged?.Invoke(idx);
    }

    public void SetIndex(int i)
    {
        idx = Mathf.Clamp(i, 0, Options.Count - 1);
        Label.Text = Options[idx];
    }
}
