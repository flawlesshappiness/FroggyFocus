public static partial class Data
{
    public static GameSaveData Game => GameProfileController.Instance.GetSelectedGameProfile();
}
