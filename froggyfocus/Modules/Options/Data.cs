public static partial class Data
{
    public static OptionsData Options => SaveDataController.Instance.Get<OptionsData>();
}
