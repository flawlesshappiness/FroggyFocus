using System.Collections.Generic;
using System.Linq;

public class FrogAppearanceData
{
    public ColorData BaseColor { get; set; } = new ColorData { R = 0.5f, G = 0.5f, B = 0.4f };
    public ColorData CoatColor { get; set; } = new ColorData { G = 0.7f };
    public ColorData PatternColor { get; set; } = new ColorData { G = 0.5f };
    public ColorData EyeColor { get; set; } = new ColorData { R = 1.0f, G = 1.0f, B = 1.0f };
    public List<FrogAppearanceAttachmentData> Attachments { get; set; } = new();

    public FrogAppearanceAttachmentData GetOrCreateAttachmentData(ItemCategory category)
    {
        var data = Attachments.FirstOrDefault(x => x.Category == category);

        if (data == null)
        {
            data = new FrogAppearanceAttachmentData { Category = category };
            if (category == ItemCategory.BodyTop) data.Type = ItemType.BodyTop_None;
            if (category == ItemCategory.BodyPattern) data.Type = ItemType.BodyPattern_None;
            if (category == ItemCategory.BodyEye) data.Type = ItemType.BodyEye_None;
            Attachments.Add(data);
        }

        return data;
    }
}
