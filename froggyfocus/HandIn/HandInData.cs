using System.Collections.Generic;

public class HandInData
{
    public string Id { get; set; }
    public bool Initialized { get; set; }
    public bool Claimed { get; set; }
    public string DateTimeNext { get; set; }
    public List<InventoryCharacterData> Requests { get; set; } = new();
    public AppearanceHatType HatUnlock { get; set; } = AppearanceHatType.None;
    public int MoneyReward { get; set; }
}
