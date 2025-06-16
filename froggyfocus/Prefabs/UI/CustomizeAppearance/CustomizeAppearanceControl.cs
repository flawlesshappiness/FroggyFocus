using Godot;
using System;
using System.Collections.Generic;

public partial class CustomizeAppearanceControl : ControlScript
{
    [Export]
    public TabContainer TabContainer;

    [Export]
    public Slider ColorRedSlider;

    [Export]
    public Slider ColorGreenSlider;

    [Export]
    public Slider ColorBlueSlider;

    [Export]
    public Button BackButton;

    [Export]
    public AppearanceButton HatButtonTemplate;

    [Export]
    public FrogCharacter Frog;

    public event Action OnBackPressed;

    public static event Action OnBodyColorChanged;
    public static event Action OnHatChanged;

    private bool loading;
    private List<AppearanceButton> hat_buttons = new();

    public override void _Ready()
    {
        base._Ready();

        ColorRedSlider.ValueChanged += _ => ColorSlider_ValueChanged();
        ColorGreenSlider.ValueChanged += _ => ColorSlider_ValueChanged();
        ColorBlueSlider.ValueChanged += _ => ColorSlider_ValueChanged();

        BackButton.Pressed += BackPressed;
    }

    protected override void Initialize()
    {
        base.Initialize();
        InitializeHats();
    }

    private void InitializeHats()
    {
        HatButtonTemplate.Hide();

        var empty_button = CreateHatButton();
        empty_button.Pressed += () => HatButtonPressed(AppearanceHatType.None);

        foreach (var info in AppearanceHatController.Instance.Collection.Resources)
        {
            var button = CreateHatButton();
            button.SetPrefab(info.Prefab);
            button.Pressed += () => HatButtonPressed(info.Type);
        }
    }

    private AppearanceButton CreateHatButton()
    {
        var button = HatButtonTemplate.Duplicate() as AppearanceButton;
        button.SetParent(HatButtonTemplate.GetParent());
        button.Show();
        return button;
    }

    protected override void OnShow()
    {
        base.OnShow();
        Load();
    }

    protected override void OnHide()
    {
        base.OnHide();
    }

    public void Load()
    {
        loading = true;

        ColorRedSlider.Value = Data.Game.FrogAppearanceData.ColorR;
        ColorGreenSlider.Value = Data.Game.FrogAppearanceData.ColorG;
        ColorBlueSlider.Value = Data.Game.FrogAppearanceData.ColorB;

        Frog.LoadAppearance();

        loading = false;
    }

    private void BackPressed()
    {
        Data.Game.Save();
        OnBackPressed?.Invoke();
    }

    private void ColorSlider_ValueChanged()
    {
        if (loading) return;

        Data.Game.FrogAppearanceData.ColorR = (float)ColorRedSlider.Value;
        Data.Game.FrogAppearanceData.ColorG = (float)ColorGreenSlider.Value;
        Data.Game.FrogAppearanceData.ColorB = (float)ColorBlueSlider.Value;

        OnBodyColorChanged?.Invoke();
    }

    private void HatButtonPressed(AppearanceHatType type)
    {
        Data.Game.FrogAppearanceData.Hat = type;

        OnHatChanged?.Invoke();
    }
}
