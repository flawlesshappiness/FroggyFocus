using Godot;
using System;
using System.Collections.Generic;

public partial class OptionsContainer : ControlScript
{
    [Export]
    public OptionsControl Options;

    [Export]
    public Slider EnvironmentSlider;

    [Export]
    public Slider CameraSensitivtySlider;

    [Export]
    public OptionsButtonControl UIScaleOptions;

    [Export]
    public OptionButton GamepadDisplayOptionButton;

    [Export]
    public OptionButton CutsceneTypeOptionButton;

    public static Action OnUIScaleChanged;
    public static Action<int> OnGamepadDisplayChanged;
    public static Action<int> OnCutsceneTypeChanged;
    public event Action BackPressed;

    private bool showing;

    public override void _Ready()
    {
        base._Ready();

        EnvironmentSlider.ValueChanged += EnvironmentVolume_ValueChanged;
        CameraSensitivtySlider.ValueChanged += CameraSensitivity_ValueChanged;
        UIScaleOptions.IndexChanged += UIScaleOptions_IndexChanged;
        GamepadDisplayOptionButton.ItemSelected += GamepadDisplayOptionButton_ItemSelected;
        CutsceneTypeOptionButton.ItemSelected += CutsceneTypeOptionButton_ItemSelected;

        OptionsController.Instance.UpdateVolume(AudioBusNames.Environment, Data.Options.EnvironmentVolume);
    }

    protected override void OnShow()
    {
        base.OnShow();

        showing = true;

        EnvironmentSlider.Value = Data.Options.EnvironmentVolume;
        CameraSensitivtySlider.Value = Data.Options.CameraSensitivity;
        UIScaleOptions.SetIndex(Data.Options.UIScaleIndex);
        GamepadDisplayOptionButton.Selected = Data.Options.GamepadDisplayIndex;
        CutsceneTypeOptionButton.Selected = Data.Options.CutsceneTypeIndex;

        showing = false;
    }

    private void EnvironmentVolume_ValueChanged(double value)
    {
        if (showing) return;

        var fvalue = Convert.ToSingle(value);
        Data.Options.EnvironmentVolume = fvalue;
        OptionsController.Instance.UpdateVolume(AudioBusNames.Environment, Data.Options.EnvironmentVolume);
    }

    private void CameraSensitivity_ValueChanged(double value)
    {
        if (showing) return;

        var fvalue = Convert.ToSingle(value);
        Data.Options.CameraSensitivity = fvalue;
    }

    private void UIScaleOptions_IndexChanged(int index)
    {
        if (showing) return;

        var scales = new List<float> { 0.8f, 0.9f, 1.0f, 1.1f, 1.2f };
        Data.Options.UIScale = scales.GetClamped(index);
        Data.Options.UIScaleIndex = index;
        OnUIScaleChanged?.Invoke();
    }

    private void GamepadDisplayOptionButton_ItemSelected(long l_index)
    {
        var index = (int)l_index;
        Data.Options.GamepadDisplayIndex = index;
        OnGamepadDisplayChanged?.Invoke(index);
    }

    private void CutsceneTypeOptionButton_ItemSelected(long l_index)
    {
        var index = (int)l_index;
        Data.Options.CutsceneTypeIndex = index;
        OnCutsceneTypeChanged?.Invoke(index);
    }
}
