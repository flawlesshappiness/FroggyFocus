public partial class AppearanceHatController : ResourceController<AppearanceHatCollection, AppearanceHatInfo>
{
    public static AppearanceHatController Instance => Singleton.Get<AppearanceHatController>();
    public override string Directory => "Appearance/Hat";
}
