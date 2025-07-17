using Godot;
using System;

public partial class OptionsContainer : Control
{
    [Export]
    public OptionsControl Options;

    [Export]
    public Slider CameraSensitivtySlider;

    public override void _Ready()
    {
        base._Ready();

        CameraSensitivtySlider.Value = Data.Options.CameraSensitivity;

        CameraSensitivtySlider.ValueChanged += CameraSensitivity_ValueChanged;
    }

    private void CameraSensitivity_ValueChanged(double value)
    {
        var fvalue = Convert.ToSingle(value);
        Data.Options.CameraSensitivity = fvalue;
    }
}
