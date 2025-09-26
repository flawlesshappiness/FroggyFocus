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
    public AppearanceContainer HatPrimaryColorContainer;

    [Export]
    public AppearanceContainer HatSecondaryColorContainer;

    [Export]
    public AppearanceContainer FaceContainer;

    [Export]
    public AppearanceContainer FacePrimaryColorContainer;

    [Export]
    public AppearanceContainer FaceSecondaryColorContainer;

    [Export]
    public FrogCharacter Frog;

    [Export]
    public Node3D PreviewRotationNode;

    public event Action OnBack;

    public static event Action OnBodyColorChanged;
    public static event Action OnHatChanged;
    public static event Action OnFaceChanged;

    private bool loading;

    private FrogAppearanceAttachmentData HatData => Data.Game.FrogAppearanceData.GetAttachmentData(ItemCategory.Hat);
    private FrogAppearanceAttachmentData FaceData => Data.Game.FrogAppearanceData.GetAttachmentData(ItemCategory.Face);

    public override void _Ready()
    {
        base._Ready();

        BodyPrimaryColorContainer.OnButtonPressed += BodyColor_Pressed;

        HatContainer.OnButtonPressed += HatButton_Pressed;
        HatPrimaryColorContainer.OnButtonPressed += HatPrimaryColor_Pressed;
        HatSecondaryColorContainer.OnButtonPressed += HatSecondaryColor_Pressed;

        FaceContainer.OnButtonPressed += FaceButton_Pressed;
        FacePrimaryColorContainer.OnButtonPressed += FacePrimaryColor_Pressed;
        FaceSecondaryColorContainer.OnButtonPressed += FaceSecondaryColor_Pressed;


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

    private void BodyColor_Pressed(AppearanceInfo info)
    {
        if (loading) return;
        Data.Game.FrogAppearanceData.BodyColor = info.Type;
        OnBodyColorChanged?.Invoke();
    }

    private void HatPrimaryColor_Pressed(AppearanceInfo info)
    {
        if (loading) return;
        HatData.PrimaryColor = info.Type;
        OnHatChanged?.Invoke();
    }

    private void HatSecondaryColor_Pressed(AppearanceInfo info)
    {
        if (loading) return;
        HatData.SecondaryColor = info.Type;
        OnHatChanged?.Invoke();
    }

    private void HatButton_Pressed(AppearanceInfo info)
    {
        if (loading) return;
        HatData.Type = info.Type;
        OnHatChanged?.Invoke();
    }

    private void FaceButton_Pressed(AppearanceInfo info)
    {
        if (loading) return;
        FaceData.Type = info.Type;
        OnFaceChanged?.Invoke();
    }

    private void FacePrimaryColor_Pressed(AppearanceInfo info)
    {
        if (loading) return;
        FaceData.PrimaryColor = info.Type;
        OnFaceChanged?.Invoke();
    }

    private void FaceSecondaryColor_Pressed(AppearanceInfo info)
    {
        if (loading) return;
        FaceData.SecondaryColor = info.Type;
        OnFaceChanged?.Invoke();
    }
}
