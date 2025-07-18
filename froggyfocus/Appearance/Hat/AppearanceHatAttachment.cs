using Godot;

public partial class AppearanceHatAttachment : AppearanceAttachment
{
    [Export]
    public AppearanceHatType Type;

    public AppearanceHatInfo Info { get; set; }

    public override void _Ready()
    {
        base._Ready();
        Info = AppearanceHatController.Instance.GetInfo(Type);
    }

    public void SetDefaultColors()
    {
        SetDefaultPrimaryColor();
        SetDefaultSecondaryColor();
    }

    public void SetDefaultPrimaryColor()
    {
        SetPrimaryColor(AppearanceColorController.Instance.GetColor(Info.DefaultPrimaryColor));
    }

    public void SetDefaultSecondaryColor()
    {
        SetSecondaryColor(AppearanceColorController.Instance.GetColor(Info.DefaultSecondaryColor));
    }
}
