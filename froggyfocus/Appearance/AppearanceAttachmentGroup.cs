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

    public void SetAttachment(ItemType type, ItemType color_primary, ItemType color_secondary)
    {
        HideAll();

        var attachment = attachments.FirstOrDefault(x => x.Info.Type == type);
        if (attachment == null) return;

        var category = attachment.Info.Category;
        var info = AppearanceController.Instance.GetInfo(type);

        var primary_color_type = color_primary == ItemType.Color_Default ? info.DefaultPrimaryColor : color_primary;
        var secondary_color_type = color_secondary == ItemType.Color_Default ? info.DefaultSecondaryColor : color_secondary;

        var primary_color = AppearanceColorController.Instance.GetColor(primary_color_type);
        var secondary_color = AppearanceColorController.Instance.GetColor(secondary_color_type);

        attachment.SetPrimaryColor(primary_color);
        attachment.SetSecondaryColor(secondary_color);
        attachment.Show();
    }
}
