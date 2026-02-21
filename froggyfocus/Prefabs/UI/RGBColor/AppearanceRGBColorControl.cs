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
        if (Category == ItemCategory.BodyBase)
        {
            Load(Data.Game.FrogAppearanceData.BaseColor.Color);
        }
        else if (Category == ItemCategory.BodyTop)
        {
            Load(Data.Game.FrogAppearanceData.CoatColor.Color);
        }
        else if (Category == ItemCategory.BodyPattern)
        {
            Load(Data.Game.FrogAppearanceData.PatternColor.Color);
        }
        else if (Category == ItemCategory.BodyEye)
        {
            Load(Data.Game.FrogAppearanceData.EyeColor.Color);
        }
        else
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
    }

    protected override void ColorChanged()
    {
        base.ColorChanged();

        if (Category == ItemCategory.BodyBase)
        {
            Data.Game.FrogAppearanceData.BaseColor.Color = Color;
        }
        else if (Category == ItemCategory.BodyTop)
        {
            Data.Game.FrogAppearanceData.CoatColor.Color = Color;
        }
        else if (Category == ItemCategory.BodyPattern)
        {
            Data.Game.FrogAppearanceData.PatternColor.Color = Color;
        }
        else if (Category == ItemCategory.BodyEye)
        {
            Data.Game.FrogAppearanceData.EyeColor.Color = Color;
        }
        else
        {
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
}
