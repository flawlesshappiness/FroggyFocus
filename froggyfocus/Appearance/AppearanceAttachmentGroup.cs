using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class AppearanceAttachmentGroup : Node3D
{
    private List<AppearanceAttachment> attachments;

    private void InitializeAttachments()
    {
        attachments ??= this.GetNodesInChildren<AppearanceAttachment>();
    }

    public void Clear()
    {
        HideAll();
    }

    private void HideAll()
    {
        InitializeAttachments();
        foreach (var attachment in attachments)
        {
            attachment.Hide();
        }
    }

    public void SetAttachment(ItemType type, Color color_primary, Color color_secondary)
    {
        InitializeAttachments();

        HideAll();

        var attachment = attachments.FirstOrDefault(x => x.Info.Type == type);
        if (attachment == null) return;

        var category = attachment.Info.Category;
        var info = AppearanceController.Instance.GetInfo(type);

        var primary_color = color_primary;
        var secondary_color = color_secondary;

        attachment.SetPrimaryColor(primary_color);
        attachment.SetSecondaryColor(secondary_color);
        attachment.Show();
    }
}
