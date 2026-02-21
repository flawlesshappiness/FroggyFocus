using System.Collections.Generic;
using System.Linq;

public class FrogAppearanceData
{
    public ColorData BaseColor { get; set; } = new ColorData { G = 255 };
    public ColorData CoatColor { get; set; } = new ColorData { G = 200 };
    public ColorData PatternColor { get; set; } = new ColorData { G = 150 };
    public ColorData EyeColor { get; set; } = new();
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
