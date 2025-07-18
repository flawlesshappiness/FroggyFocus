public class FrogAppearanceData
{
    public AppearanceColorType BodyColor { get; set; }
    public bool HatPrimaryColorDefault { get; set; } = true;
    public bool HatSecondaryColorDefault { get; set; } = true;
    public AppearanceColorType HatPrimaryColor { get; set; }
    public AppearanceColorType HatSecondaryColor { get; set; }
    public AppearanceHatType Hat { get; set; }
}
