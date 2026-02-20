using Godot;

public partial class AppearanceRGBColorControl : RGBColorControl
{
    [Export]
    public ItemCategory Category;

    [Export]
    public bool IsPrimary;

    public override void _Ready()
    {
        base._Ready();
        GameProfileController.Instance.OnGameProfileSelected += ProfileSelected;
    }

    private void ProfileSelected(int i)
    {
        var data = Data.Game.FrogAppearanceData.GetOrCreateAttachmentData(Category);
        if (IsPrimary)
        {
            Load(data.PrimaryColor);
        }
        else
        {
            Load(data.SecondaryColor);
        }
    }

    protected override void ColorChanged()
    {
        base.ColorChanged();
        var data = Data.Game.FrogAppearanceData.GetOrCreateAttachmentData(Category);
        var c = Color;

        if (IsPrimary)
        {
            data.PrimaryR = c.R;
            data.PrimaryG = c.G;
            data.PrimaryB = c.B;
        }
        else
        {
            data.SecondaryR = c.R;
            data.SecondaryG = c.G;
            data.SecondaryB = c.B;
        }
    }
}
