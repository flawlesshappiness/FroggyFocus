public class FrogAppearanceAttachmentData
{
    public ItemCategory Category { get; set; }
    public ItemType Type { get; set; }
    public ItemType PrimaryColor { get; set; } = ItemType.Color_Default;
    public ItemType SecondaryColor { get; set; } = ItemType.Color_Default;
}
