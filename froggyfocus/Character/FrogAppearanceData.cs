using System.Collections.Generic;
using System.Linq;

public class FrogAppearanceData
{
    public ItemType BodyColor { get; set; } = ItemType.Color_Default;
    public List<FrogAppearanceAttachmentData> Attachments { get; set; } = new();

    public FrogAppearanceAttachmentData GetOrCreateAttachmentData(ItemCategory category)
    {
        var data = Attachments.FirstOrDefault(x => x.Category == category);

        if (data == null)
        {
            data = new FrogAppearanceAttachmentData { Category = category };
            Attachments.Add(data);
        }

        return data;
    }
}
