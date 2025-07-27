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

    public static Action OnUIScaleChanged;

    private bool showing;

    public override void _Ready()
    {
        base._Ready();

        EnvironmentSlider.ValueChanged += EnvironmentVolume_ValueChanged;
        CameraSensitivtySlider.ValueChanged += CameraSensitivity_ValueChanged;
        UIScaleOptions.IndexChanged += UIScaleOptions_IndexChanged;

        OptionsController.Instance.UpdateVolume(AudioBusNames.Environment, Data.Options.EnvironmentVolume);
    }

    protected override void OnShow()
    {
        base.OnShow();

        showing = true;

        EnvironmentSlider.Value = Data.Options.EnvironmentVolume;
        CameraSensitivtySlider.Value = Data.Options.CameraSensitivity;
        UIScaleOptions.SetIndex(Data.Options.UIScaleIndex);

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
}
