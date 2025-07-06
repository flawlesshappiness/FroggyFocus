using System.Collections.Generic;

public class AppearanceData
{
    public List<AppearanceHatType> UnlockedHats { get; set; } = new() { };
    public List<AppearanceHatType> PurchasedHats { get; set; } = new() { AppearanceHatType.None };
    public List<AppearanceColorType> UnlockedColors { get; set; } = new() { };
    public List<AppearanceColorType> PurchasedColors { get; set; } = new() { AppearanceColorType.Green };
}
