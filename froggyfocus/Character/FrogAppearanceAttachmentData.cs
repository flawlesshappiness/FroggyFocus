using Godot;

public class FrogAppearanceAttachmentData
{
    public ItemCategory Category { get; set; }
    public ItemType Type { get; set; }
    public float PrimaryR { get; set; }
    public float PrimaryG { get; set; }
    public float PrimaryB { get; set; }
    public float SecondaryR { get; set; }
    public float SecondaryG { get; set; }
    public float SecondaryB { get; set; }

    public Color PrimaryColor => new Color(PrimaryR, PrimaryG, PrimaryB);
    public Color SecondaryColor => new Color(SecondaryR, SecondaryG, SecondaryB);
}
