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
        ColorTab.HatSecondaryColorContainer.OnColorPressed += HatSecondaryColor_Pressed;

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

    private void BodyColor_Pressed(AppearanceColorType type)
    {
        if (loading) return;
        Data.Game.FrogAppearanceData.BodyColor = type;
        OnBodyColorChanged?.Invoke();
    }

    private void HatPrimaryColor_Pressed(AppearanceColorType type)
    {
        if (loading) return;
        Data.Game.FrogAppearanceData.HatPrimaryColor = type;
        OnHatChanged?.Invoke();
    }

    private void HatSecondaryColor_Pressed(AppearanceColorType type)
    {
        if (loading) return;
        Data.Game.FrogAppearanceData.HatSecondaryColor = type;
        OnHatChanged?.Invoke();
    }

    private void HatButton_Pressed(AppearanceHatInfo info)
    {
        if (loading) return;
        Data.Game.FrogAppearanceData.Hat = info?.Type ?? AppearanceHatType.None;
        OnHatChanged?.Invoke();
    }
}
