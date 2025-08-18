using System.Collections.Generic;

public partial class GameSaveData
{
    public bool LocationsInitialized { get; set; }
    public List<LocationData> Locations { get; set; } = new();
}
