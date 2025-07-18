using Godot;
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
    public CustomizeAppearanceColorTab ColorTab;

    [Export]
    public HatsContainer HatsContainer;

    [Export]
    public FrogCharacter Frog;

    [Export]
    public Node3D PreviewRotationNode;

    public event Action OnBack;

    public static event Action OnBodyColorChanged;
    public static event Action OnHatChanged;

    private bool loading;

    public override void _Ready()
    {
        base._Ready();

        HatsContainer.OnButtonPressed += HatButton_Pressed;

        ColorTab.BodyColorContainer.OnColorPressed += BodyColor_Pressed;

        ColorTab.HatPrimaryColorContainer.OnColorPressed += HatPrimaryColor_Pressed;
        ColorTab.HatPrimaryColorContainer.OnDefaultColorPressed += HatPrimaryDefaultColor_Pressed;
        ColorTab.HatSecondaryColorContainer.OnColorPressed += HatSecondaryColor_Pressed;
        ColorTab.HatSecondaryColorContainer.OnDefaultColorPressed += HatSecondaryDefaultColor_Pressed;

        PreviewRotationSlider.ValueChanged += PreviewRotationSlider_ValueChanged;
        PreviewRotationSlider_ValueChanged(PreviewRotationSlider.Value);

        BackButton.Pressed += BackPressed;
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (Input.IsActionJustReleased("ui_cancel") && IsVisibleInTree())
        {
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

    private void BodyColor_Pressed(AppearanceColorInfo info)
    {
        if (loading) return;
        Data.Game.FrogAppearanceData.BodyColor = info.Type;
        OnBodyColorChanged?.Invoke();
    }

    private void HatPrimaryColor_Pressed(AppearanceColorInfo info)
    {
        if (loading) return;
        Data.Game.FrogAppearanceData.HatPrimaryColor = info.Type;
        Data.Game.FrogAppearanceData.HatPrimaryColorDefault = false;
        OnHatChanged?.Invoke();
    }

    private void HatPrimaryDefaultColor_Pressed()
    {
        if (loading) return;
        Data.Game.FrogAppearanceData.HatPrimaryColorDefault = true;
        OnHatChanged?.Invoke();
    }

    private void HatSecondaryColor_Pressed(AppearanceColorInfo info)
    {
        if (loading) return;
        Data.Game.FrogAppearanceData.HatSecondaryColor = info.Type;
        Data.Game.FrogAppearanceData.HatSecondaryColorDefault = false;
        OnHatChanged?.Invoke();
    }

    private void HatSecondaryDefaultColor_Pressed()
    {
        if (loading) return;
        Data.Game.FrogAppearanceData.HatSecondaryColorDefault = true;
        OnHatChanged?.Invoke();
    }

    private void HatButton_Pressed(AppearanceHatInfo info)
    {
        if (loading) return;
        Data.Game.FrogAppearanceData.Hat = info?.Type ?? AppearanceHatType.None;
        OnHatChanged?.Invoke();
    }
}
