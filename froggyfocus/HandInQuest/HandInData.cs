using System.Collections.Generic;

public class HandInData : QuestData
{
    public List<InventoryCharacterData> Requests { get; set; } = new();
    public int MoneyReward { get; set; }
    public int ClaimedCount { get; set; }
    public bool Claimed { get; set; }
}
