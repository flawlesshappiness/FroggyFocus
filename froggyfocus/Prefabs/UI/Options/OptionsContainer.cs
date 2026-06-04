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
    public Slider DeadzoneSlider;

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

    [Export]
    public OptionButton ForcedDisplayOptionButton;

    public static Action OnUIScaleChanged;
    public static Action OnGamepadDisplayChanged;
    public event Action BackPressed;

    private bool showing;
    private List<ControlMap> control_maps = new();

    private abstract class ControlMap
    {
        public abstract void UpdateControl();
    }

    private class OptionButtonMap : ControlMap
    {
        public OptionButton OptionButton { get; set; }
        public Func<int> Get { get; set; }
        public Action<int> Set { get; set; }

        private bool updating;

        public OptionButtonMap(OptionButton button)
        {
            OptionButton = button;
            OptionButton.ItemSelected += ItemSelected;
        }

        private void ItemSelected(long l)
        {
            if (updating) return;
            Set((int)l);
        }

        public override void UpdateControl()
        {
            updating = true;
            OptionButton.Selected = Get();
            updating = false;
        }
    }

    private class SliderMap : ControlMap
    {
        public Slider Slider { get; set; }
        public Func<float> Get { get; set; }
        public Action<float> Set { get; set; }

        private bool updating;

        public SliderMap(Slider slider)
        {
            Slider = slider;
            Slider.ValueChanged += ValueChanged;
        }

        private void ValueChanged(double d)
        {
            if (updating) return;
            Set((float)d);
        }

        public override void UpdateControl()
        {
            updating = true;
            Slider.Value = Get();
            updating = false;
        }
    }

    public override void _Ready()
    {
        base._Ready();

        UIScaleOptions.IndexChanged += UIScaleOptions_IndexChanged;

        InitializeControlMaps();
        OptionsController.Instance.UpdateVolume(AudioBusNames.Environment, Data.Options.EnvironmentVolume);
        UpdateFadePlantsOption();
    }

    private void InitializeControlMaps()
    {
        // OptionButtons
        control_maps.Add(new OptionButtonMap(GamepadDisplayOptionButton)
        {
            Get = () => Data.Options.GamepadDisplayIndex,
            Set = v =>
            {
                Data.Options.GamepadDisplayIndex = v;
                OnGamepadDisplayChanged?.Invoke();
            }
        });

        control_maps.Add(new OptionButtonMap(CutsceneTypeOptionButton)
        {
            Get = () => Data.Options.CutsceneTypeIndex,
            Set = v => Data.Options.CutsceneTypeIndex = v
        });

        control_maps.Add(new OptionButtonMap(FadePlantsOptionButton)
        {
            Get = () => Data.Options.FadePlantsIndex,
            Set = v =>
            {
                Data.Options.FadePlantsIndex = v;
                UpdateFadePlantsOption();
            }
        });

        control_maps.Add(new OptionButtonMap(CatchBugTutorialOptionButton)
        {
            Get = () => Data.Options.CatchTutorialEnabled ? 0 : 1,
            Set = v => Data.Options.CatchTutorialEnabled = v == 0
        });

        control_maps.Add(new OptionButtonMap(CollectBugSoundOptionButton)
        {
            Get = () => Data.Options.CollectBugSoundEnabled ? 0 : 1,
            Set = v => Data.Options.CollectBugSoundEnabled = v == 0
        });

        control_maps.Add(new OptionButtonMap(JumpChargeEffectOptionButton)
        {
            Get = () => Data.Options.JumpChargeEffectEnabled ? 0 : 1,
            Set = v => Data.Options.JumpChargeEffectEnabled = v == 0
        });

        control_maps.Add(new OptionButtonMap(ForcedDisplayOptionButton)
        {
            Get = () => Data.Options.ForcedInputDisplayIndex,
            Set = v =>
            {
                Data.Options.ForcedInputDisplayIndex = v;
                OnGamepadDisplayChanged?.Invoke();
            }
        });

        // Sliders
        control_maps.Add(new SliderMap(EnvironmentSlider)
        {
            Get = () => Data.Options.EnvironmentVolume,
            Set = v => Data.Options.EnvironmentVolume = v
        });

        control_maps.Add(new SliderMap(CameraSensitivtySlider)
        {
            Get = () => Data.Options.CameraSensitivity,
            Set = v => Data.Options.CameraSensitivity = v
        });

        control_maps.Add(new SliderMap(DeadzoneSlider)
        {
            Get = () => Data.Options.GamepadDeadZone,
            Set = v => Data.Options.GamepadDeadZone = v
        });
    }

    protected override void OnShow()
    {
        base.OnShow();

        showing = true;

        UIScaleOptions.SetIndex(Data.Options.UIScaleIndex);

        control_maps.ForEach(x => x.UpdateControl());

        showing = false;
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
