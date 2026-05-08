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

    [Export]
    public OptionButton FadePlantsOptionButton;

    [Export]
    public OptionButton CatchBugTutorialOptionButton;

    [Export]
    public OptionButton CollectBugSoundOptionButton;

    [Export]
    public OptionButton JumpChargeEffectOptionButton;

    public static Action OnUIScaleChanged;
    public static Action<int> OnGamepadDisplayChanged;
    public event Action BackPressed;

    private bool showing;
    private List<OptionButtonMap> option_buttons = new();

    private class OptionButtonMap
    {
        public OptionButton OptionButton { get; set; }
        public Func<int> Get { get; set; }
        public Action<int> Set { get; set; }

        public OptionButtonMap(OptionButton button)
        {
            OptionButton = button;
            OptionButton.ItemSelected += SetValue;
        }

        private void SetValue(long l)
        {
            Set((int)l);
        }
    }

    public override void _Ready()
    {
        base._Ready();

        EnvironmentSlider.ValueChanged += EnvironmentVolume_ValueChanged;
        CameraSensitivtySlider.ValueChanged += CameraSensitivity_ValueChanged;
        UIScaleOptions.IndexChanged += UIScaleOptions_IndexChanged;

        InitializeOptionButtons();
        OptionsController.Instance.UpdateVolume(AudioBusNames.Environment, Data.Options.EnvironmentVolume);
        UpdateFadePlantsOption();
    }

    private void InitializeOptionButtons()
    {
        option_buttons.Add(new OptionButtonMap(GamepadDisplayOptionButton)
        {
            Get = () => Data.Options.GamepadDisplayIndex,
            Set = i =>
            {
                Data.Options.GamepadDisplayIndex = i;
                OnGamepadDisplayChanged?.Invoke(i);
            }
        });

        option_buttons.Add(new OptionButtonMap(CutsceneTypeOptionButton)
        {
            Get = () => Data.Options.CutsceneTypeIndex,
            Set = i => Data.Options.CutsceneTypeIndex = i
        });

        option_buttons.Add(new OptionButtonMap(FadePlantsOptionButton)
        {
            Get = () => Data.Options.FadePlantsIndex,
            Set = i =>
            {
                Data.Options.FadePlantsIndex = i;
                UpdateFadePlantsOption();
            }
        });

        option_buttons.Add(new OptionButtonMap(CatchBugTutorialOptionButton)
        {
            Get = () => Data.Options.CatchTutorialEnabled ? 0 : 1,
            Set = i => Data.Options.CatchTutorialEnabled = i == 0
        });

        option_buttons.Add(new OptionButtonMap(CollectBugSoundOptionButton)
        {
            Get = () => Data.Options.CollectBugSoundEnabled ? 0 : 1,
            Set = i => Data.Options.CollectBugSoundEnabled = i == 0
        });

        option_buttons.Add(new OptionButtonMap(JumpChargeEffectOptionButton)
        {
            Get = () => Data.Options.JumpChargeEffectEnabled ? 0 : 1,
            Set = i => Data.Options.JumpChargeEffectEnabled = i == 0
        });
    }

    protected override void OnShow()
    {
        base.OnShow();

        showing = true;

        EnvironmentSlider.Value = Data.Options.EnvironmentVolume;
        CameraSensitivtySlider.Value = Data.Options.CameraSensitivity;
        UIScaleOptions.SetIndex(Data.Options.UIScaleIndex);

        foreach (var map in option_buttons)
        {
            map.OptionButton.Selected = map.Get();
        }

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

    private void UpdateFadePlantsOption()
    {
        var i = Data.Options.FadePlantsIndex;
        RenderingServer.GlobalShaderParameterSet("setting_hide_mesh_near_view", i == 0 || i == 2);
        RenderingServer.GlobalShaderParameterSet("setting_hide_mesh_near_player", i == 1 || i == 2);
    }
}
