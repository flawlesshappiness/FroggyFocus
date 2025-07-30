using System.Collections.Generic;

public partial class GameSaveData
{
    public bool UpgradesInitialized { get; set; }
    public List<UpgradeData> Upgrades { get; set; } = new();
}
