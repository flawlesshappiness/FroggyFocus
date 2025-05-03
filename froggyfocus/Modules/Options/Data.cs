public static partial class Data
{
    public static OptionsData Options => _options ?? (_options = SaveDataController.Instance.GetOrCreate<OptionsData>());
    private static OptionsData _options;
}
