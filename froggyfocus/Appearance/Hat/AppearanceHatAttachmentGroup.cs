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

        hat.Show();
    }
}
