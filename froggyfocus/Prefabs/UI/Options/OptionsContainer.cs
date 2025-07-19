using Godot;
using System;

public partial class OptionsContainer : Control
{
    [Export]
    public OptionsControl Options;

    [Export]
    public Slider EnvironmentSlider;

    [Export]
    public Slider CameraSensitivtySlider;

    public override void _Ready()
    {
        base._Ready();

        EnvironmentSlider.Value = Data.Options.EnvironmentVolume;
        CameraSensitivtySlider.Value = Data.Options.CameraSensitivity;

        EnvironmentSlider.ValueChanged += EnvironmentVolume_ValueChanged;
        CameraSensitivtySlider.ValueChanged += CameraSensitivity_ValueChanged;

        OptionsController.Instance.UpdateVolume(AudioBusNames.Environment, Data.Options.EnvironmentVolume);
    }

    private void EnvironmentVolume_ValueChanged(double value)
    {
        var fvalue = Convert.ToSingle(value);
        Data.Options.EnvironmentVolume = fvalue;
        OptionsController.Instance.UpdateVolume(AudioBusNames.Environment, Data.Options.EnvironmentVolume);
    }

    private void CameraSensitivity_ValueChanged(double value)
    {
        var fvalue = Convert.ToSingle(value);
        Data.Options.CameraSensitivity = fvalue;
    }
}
