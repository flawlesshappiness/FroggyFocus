public static partial class Data
{
    public static GameSaveData Game => SaveDataController.Instance.Get<GameSaveData>();
}
