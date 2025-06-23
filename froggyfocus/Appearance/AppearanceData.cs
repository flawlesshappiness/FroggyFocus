using System.Collections.Generic;

public class AppearanceData
{
    public List<AppearanceHatType> UnlockedHats { get; set; } = new() { AppearanceHatType.None };
    public List<AppearanceColorType> UnlockedColors { get; set; } = new() { AppearanceColorType.Green };
}
