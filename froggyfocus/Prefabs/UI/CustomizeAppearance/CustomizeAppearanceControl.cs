using Godot;
using System;

public partial class CustomizeAppearanceControl : ControlScript
{
    [Export]
    public Slider ColorRedSlider;

    [Export]
    public Slider ColorGreenSlider;

    [Export]
    public Slider ColorBlueSlider;

    [Export]
    public Button BackButton;

    [Export]
    public FrogCharacter Frog;

    public event Action OnBackPressed;

    public static event Action OnBodyColorChanged;

    private bool loading;

    public override void _Ready()
    {
        base._Ready();

        ColorRedSlider.ValueChanged += _ => ColorSlider_ValueChanged();
        ColorGreenSlider.ValueChanged += _ => ColorSlider_ValueChanged();
        ColorBlueSlider.ValueChanged += _ => ColorSlider_ValueChanged();

        BackButton.Pressed += BackPressed;
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
}
