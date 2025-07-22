using Godot;
using System;

public partial class OptionsContainer : ControlScript
{
    [Export]
    public OptionsControl Options;

    [Export]
    public Slider EnvironmentSlider;

    [Export]
    public Slider CameraSensitivtySlider;

    [Export]
    public Slider UIScaleSlider;

    public static Action OnUIScaleChanged;

    private bool showing;

    public override void _Ready()
    {
        base._Ready();

        EnvironmentSlider.ValueChanged += EnvironmentVolume_ValueChanged;
        CameraSensitivtySlider.ValueChanged += CameraSensitivity_ValueChanged;
        UIScaleSlider.ValueChanged += UIScaleSlider_ValueChanged;

        OptionsController.Instance.UpdateVolume(AudioBusNames.Environment, Data.Options.EnvironmentVolume);
    }

    protected override void OnShow()
    {
        base.OnShow();

        showing = true;

        EnvironmentSlider.Value = Data.Options.EnvironmentVolume;
        CameraSensitivtySlider.Value = Data.Options.CameraSensitivity;
        UIScaleSlider.Value = Data.Options.UIScale;

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

    private void UIScaleSlider_ValueChanged(double value)
    {
        if (showing) return;

        var fvalue = Convert.ToSingle(value);
        Data.Options.UIScale = fvalue;
        OnUIScaleChanged?.Invoke();
    }
}
