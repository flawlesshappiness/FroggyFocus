using Godot;
using Godot.Collections;
using System;

public partial class CustomizeAppearanceControl : ControlScript
{
    [Export]
    public TabContainer TabContainer;

    [Export]
    public Slider PreviewRotationSlider;

    [Export]
    public Button BackButton;

    [Export]
    public FrogCharacter Frog;

    [Export]
    public Node3D PreviewRotationNode;

    [Export]
    public Array<AppearanceContainer> AppearanceContainers;

    [Export]
    public Array<AppearanceRGBColorControl> ColorControls;

    public event Action OnBack;

    public static event Action OnAppearanceChanged;

    private bool loading;

    public override void _Ready()
    {
        base._Ready();

        foreach (var container in AppearanceContainers)
        {
            container.OnButtonPressed += AppearanceButton_Pressed;
        }

        foreach (var control in ColorControls)
        {
            control.OnColorChanged += ColorControl_ColorChanged;
        }

        PreviewRotationSlider.ValueChanged += PreviewRotationSlider_ValueChanged;
        PreviewRotationSlider_ValueChanged(PreviewRotationSlider.Value);

        BackButton.Pressed += BackPressed;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);

        if (Input.IsActionJustReleased("ui_cancel") && IsVisibleInTree())
        {
            GetViewport().SetInputAsHandled();
            BackPressed();
        }
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

        Frog.LoadAppearance();

        loading = false;
    }

    private void BackPressed()
    {
        Data.Game.Save();
        OnBack?.Invoke();
    }

    private void PreviewRotationSlider_ValueChanged(double dvalue)
    {
        var value = Convert.ToSingle(dvalue);
        PreviewRotationNode.RotationDegrees = new Vector3(0, value, 0);
    }

    private void AppearanceButton_Pressed(AppearanceInfo info)
    {
        if (loading) return;
        OnAppearanceChanged?.Invoke();
    }

    private void ColorControl_ColorChanged(Color color)
    {
        if (loading) return;
        OnAppearanceChanged?.Invoke();
    }
}
