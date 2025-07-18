using Godot;
using Godot.Collections;
using System.Linq;

public partial class AppearanceHatAttachmentGroup : Node3D
{
    [Export]
    public Array<AppearanceHatAttachment> Hats = new();

    public void Clear()
    {
        HideAllHats();
    }

    private void HideAllHats()
    {
        Hats.ForEach(x => x.Hide());
    }

    public void SetHat(AppearanceHatType type)
    {
        HideAllHats();

        var hat = Hats.FirstOrDefault(x => x.Type == type);
        if (hat == null) return;

        var primary_color_type = Data.Game.FrogAppearanceData.HatPrimaryColorDefault ? hat.Info.DefaultPrimaryColor : Data.Game.FrogAppearanceData.HatPrimaryColor;
        var secondary_color_type = Data.Game.FrogAppearanceData.HatSecondaryColorDefault ? hat.Info.DefaultSecondaryColor : Data.Game.FrogAppearanceData.HatSecondaryColor;

        var primary_color = AppearanceColorController.Instance.GetColor(primary_color_type);
        var secondary_color = AppearanceColorController.Instance.GetColor(secondary_color_type);

        hat.SetPrimaryColor(primary_color);
        hat.SetSecondaryColor(secondary_color);
        hat.Show();
    }
}
