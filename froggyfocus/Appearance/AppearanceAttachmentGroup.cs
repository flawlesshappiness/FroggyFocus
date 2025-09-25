using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class AppearanceAttachmentGroup : Node3D
{
    private List<AppearanceAttachment> attachments = new();

    public override void _Ready()
    {
        base._Ready();
        attachments = this.GetNodesInChildren<AppearanceAttachment>();
    }

    public void Clear()
    {
        HideAll();
    }

    private void HideAll()
    {
        attachments.ForEach(x => x.Hide());
    }

    public void SetAttachment(ItemType type)
    {
        HideAll();

        var attachment = attachments.FirstOrDefault(x => x.Info.Type == type);
        if (attachment == null) return;

        var category = attachment.Info.Category;
        var info = AppearanceController.Instance.GetInfo(type);
        var data = Data.Game.FrogAppearanceData.GetAttachmentData(category);

        var primary_color_type = data.PrimaryColor == ItemType.Color_Default ? info.DefaultPrimaryColor : data.PrimaryColor;
        var secondary_color_type = data.SecondaryColor == ItemType.Color_Default ? info.DefaultSecondaryColor : data.SecondaryColor;

        var primary_color = AppearanceColorController.Instance.GetColor(primary_color_type);
        var secondary_color = AppearanceColorController.Instance.GetColor(secondary_color_type);

        attachment.SetPrimaryColor(primary_color);
        attachment.SetSecondaryColor(secondary_color);
        attachment.Show();
    }
}
