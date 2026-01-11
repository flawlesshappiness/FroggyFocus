using Godot;
using System;

public partial class OptionsControl : ControlScript
{
    [Export]
    public Button BackButton;

    [Export]
    public TabContainer Tabs;

    [Export]
    public Slider MasterSlider;

    [Export]
    public Slider SFXSlider;

    [Export]
    public Slider BGMSlider;

    [Export]
    public OptionButton WindowModeDropdown;

    [Export]
    public Control Resolution;

    [Export]
    public OptionButton ResolutionDropdown;

    [Export]
    public OptionButton VSyncDropdown;

    [Export]
    public OptionButton FPSLimitDropdown;

    [Export]
    public OptionsKeys Keys;

    public event Action BackPressed;

    public override void _Ready()
    {
        base._Ready();

        WindowMode_AddItems();
        Resolution_AddItems();
        Resolution_UpdateVisible();
        VSync_AddItems();
        FPSLimit_AddItems();

        MasterSlider.Value = Data.Options.VolumeMaster;
        SFXSlider.Value = Data.Options.VolumeSFX;
        BGMSlider.Value = Data.Options.VolumeBGM;
        WindowModeDropdown.Selected = Data.Options.WindowMode;
        ResolutionDropdown.Selected = Data.Options.Resolution;
        VSyncDropdown.Selected = Data.Options.VSync;
        FPSLimitDropdown.Selected = Data.Options.FPSLimit;

        MasterSlider.ValueChanged += MasterSlider_ValueChanged;
        SFXSlider.ValueChanged += SFXSlider_ValueChanged;
        BGMSlider.ValueChanged += BGMSlider_ValueChanged;
        WindowModeDropdown.ItemSelected += WindowMode_SelectionChanged;
        ResolutionDropdown.ItemSelected += Resolution_SelectionChanged;
        VSyncDropdown.ItemSelected += VSync_SelectionChanged;
        FPSLimitDropdown.ItemSelected += FPSLimit_SelectionChanged;

        Keys.OnRebindStart += RebindStart;
        Keys.OnRebindEnd += RebindEnd;

        BackButton.Pressed += BackButton_Pressed;
    }

    protected override void OnShow()
    {
        base.OnShow();

        Tabs.CurrentTab = 0;
        Keys.UpdateAllKeyStrings();
        Keys.UpdateDuplicateWarnings();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);

        if (Input.IsActionJustReleased("ui_cancel") && IsVisibleInTree())
        {
            if (Keys.IsRebinding) return;
            GetViewport().SetInputAsHandled();
            BackButton_Pressed();
        }
    }

    private void MasterSlider_ValueChanged(double v)
    {
        var f = Convert.ToSingle(v);
        OptionsController.Instance.UpdateVolume("Master", f);
        Data.Options.VolumeMaster = f;
    }

    private void SFXSlider_ValueChanged(double v)
    {
        var f = Convert.ToSingle(v);
        OptionsController.Instance.UpdateVolume("SFX", f);
        Data.Options.VolumeSFX = f;
    }

    private void BGMSlider_ValueChanged(double v)
    {
        var f = Convert.ToSingle(v);
        OptionsController.Instance.UpdateVolume("BGM", f);
        Data.Options.VolumeBGM = f;
    }

    private void WindowMode_AddItems()
    {
        foreach (var mode in OptionsController.WindowModes)
        {
            var item = mode switch
            {
                Window.ModeEnum.Windowed => "Windowed",
                Window.ModeEnum.ExclusiveFullscreen => "Fullscreen",
                _ => ""
            };

            WindowModeDropdown.AddItem(item);
        }
    }

    private void WindowMode_SelectionChanged(long index)
    {
        var i = (int)index;
        if (OptionsController.WindowModes.GetClamped(i) == Window.ModeEnum.Windowed)
        {
            OptionsController.Instance.UpdateResolution(i);
        }

        OptionsController.Instance.UpdateWindowMode(i);
        Data.Options.WindowMode = i;
        Resolution_UpdateVisible();
    }

    private void Resolution_UpdateVisible()
    {
        var mode = OptionsController.WindowModes.GetClamped(Data.Options.WindowMode);
        Resolution.Visible = mode == Window.ModeEnum.Windowed;
    }

    private void Resolution_AddItems()
    {
        foreach (var res in OptionsController.Resolutions)
        {
            var item = $"{res.X}x{res.Y}";
            ResolutionDropdown.AddItem(item);
        }
    }

    private void Resolution_SelectionChanged(long index)
    {
        var i = (int)index;
        OptionsController.Instance.UpdateResolution(i);
        Data.Options.Resolution = i;
    }

    private void VSync_AddItems()
    {
        foreach (var mode in OptionsController.VSyncModes)
        {
            var item = mode switch
            {
                DisplayServer.VSyncMode.Mailbox => "Fast",
                _ => mode.ToString()
            };

            VSyncDropdown.AddItem(item);
        }
    }

    private void VSync_SelectionChanged(long index)
    {
        var i = (int)index;
        OptionsController.Instance.UpdateVsync(i);
        Data.Options.VSync = i;
    }

    private void FPSLimit_AddItems()
    {
        foreach (var mode in OptionsController.FPSLimits)
        {
            var item = mode switch
            {
                0 => "Unlimited",
                _ => mode.ToString()
            };

            FPSLimitDropdown.AddItem(item);
        }
    }

    private void FPSLimit_SelectionChanged(long index)
    {
        var i = (int)index;
        OptionsController.Instance.UpdateFPSLimit(i);
        Data.Options.FPSLimit = i;
    }

    private void RebindStart()
    {
        SetTabsEnabled(false);
    }

    private void RebindEnd()
    {
        SetTabsEnabled(true);
    }

    private void SetTabsEnabled(bool enabled)
    {
        for (int i = 0; i < Tabs.GetTabCount(); i++)
        {
            Tabs.SetTabDisabled(i, !enabled);
        }
    }

    private void BackButton_Pressed()
    {
        Data.Options.Save();
        BackPressed?.Invoke();
    }
}