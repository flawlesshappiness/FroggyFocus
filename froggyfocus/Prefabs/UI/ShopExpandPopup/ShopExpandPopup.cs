using Godot;

public partial class ShopExpandPopup : PopupControl
{
    [Export]
    public ItemSubViewport ItemSubViewport;

    [Export]
    public Label NameLabel;

    [Export]
    public Label CostLabel;

    [Export]
    public Button OkButton;

    [Export]
    public AudioStreamPlayer SfxFanfare;

    public override void _Ready()
    {
        base._Ready();
        OkButton.Pressed += OkButton_Pressed;

        ItemSubViewport.SetCameraFront();
        ItemSubViewport.SetAnimationSpin();
    }

    public void SetHat(AppearanceHatType type)
    {
        var info = AppearanceHatController.Instance.GetInfo(type);
        SetHat(info);
    }

    public void SetHat(AppearanceHatInfo info)
    {
        ItemSubViewport.SetHat(info);
        NameLabel.Text = info.Name;
        CostLabel.Text = info.Price.ToString();
    }

    private void OkButton_Pressed()
    {
        ClosePopup();
    }

    protected override void PopupShown()
    {
        base.PopupShown();
        SfxFanfare.Play();
    }
}
