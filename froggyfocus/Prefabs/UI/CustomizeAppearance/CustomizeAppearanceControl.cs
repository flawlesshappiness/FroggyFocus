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
    public AppearanceContainer BodyPrimaryColorContainer;

    [Export]
    public AppearanceContainer HatContainer;

    [Export]
    public AppearanceColorControl HatColorControl;

    [Export]
    public AppearanceContainer FaceContainer;

    [Export]
    public AppearanceColorControl FaceColorControl;

    [Export]
    public AppearanceContainer ParticlesContainer;

    [Export]
    public AppearanceColorControl ParticlesColorControl;

    [Export]
    public FrogCharacter Frog;

    [Export]
    public Node3D PreviewRotationNode;

    public event Action OnBack;

    public static event Action OnBodyColorChanged;
    public static event Action OnHatChanged;
    public static event Action OnFaceChanged;
    public static event Action OnParticlesChanged;

    private bool loading;

    private FrogAppearanceAttachmentData HatData => Data.Game.FrogAppearanceData.GetOrCreateAttachmentData(ItemCategory.Hat);
    private FrogAppearanceAttachmentData FaceData => Data.Game.FrogAppearanceData.GetOrCreateAttachmentData(ItemCategory.Face);
    private FrogAppearanceAttachmentData ParticlesData => Data.Game.FrogAppearanceData.GetOrCreateAttachmentData(ItemCategory.Particles);

    public override void _Ready()
    {
        base._Ready();

        BodyPrimaryColorContainer.OnButtonPressed += BodyColor_Pressed;

        HatContainer.OnButtonPressed += HatButton_Pressed;
        HatColorControl.OnColorSelected += HatColor_Selected;

        FaceContainer.OnButtonPressed += FaceButton_Pressed;
        FaceColorControl.OnColorSelected += FaceColor_Selected;

        ParticlesContainer.OnButtonPressed += ParticlesButton_Pressed;
        ParticlesColorControl.OnColorSelected += ParticlesColor_Selected;

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

    private void BodyColor_Pressed(AppearanceInfo info)
    {
        if (loading) return;
        Data.Game.FrogAppearanceData.BodyColor = info.Type;
        OnBodyColorChanged?.Invoke();
    }

    private void HatColor_Selected(AppearanceInfo info)
    {
        if (loading) return;
        OnHatChanged?.Invoke();
    }

    private void HatButton_Pressed(AppearanceInfo info)
    {
        if (loading) return;
        HatData.Type = info.Type;
        OnHatChanged?.Invoke();

        HatColorControl.SetSecondaryEnabled(info.HasSecondaryColor);
    }

    private void FaceButton_Pressed(AppearanceInfo info)
    {
        if (loading) return;
        FaceData.Type = info.Type;
        OnFaceChanged?.Invoke();

        FaceColorControl.SetSecondaryEnabled(info.HasSecondaryColor);
    }

    private void FaceColor_Selected(AppearanceInfo info)
    {
        if (loading) return;
        OnFaceChanged?.Invoke();
    }

    private void ParticlesButton_Pressed(AppearanceInfo info)
    {
        if (loading) return;
        ParticlesData.Type = info.Type;
        OnParticlesChanged?.Invoke();

        ParticlesColorControl.SetSecondaryEnabled(info.HasSecondaryColor);
    }

    private void ParticlesColor_Selected(AppearanceInfo info)
    {
        if (loading) return;
        OnParticlesChanged?.Invoke();
    }
}
