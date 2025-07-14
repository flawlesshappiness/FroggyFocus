using System.Collections.Generic;

public class HandInData : QuestData
{
    public List<InventoryCharacterData> Requests { get; set; } = new();
    public AppearanceHatType HatUnlock { get; set; } = AppearanceHatType.None;
    public int MoneyReward { get; set; }
}
