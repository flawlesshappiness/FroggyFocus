using System;
using System.Linq;

public partial class AppearanceHatController : ResourceController<AppearanceHatCollection, AppearanceHatInfo>
{
    public static AppearanceHatController Instance => Singleton.Get<AppearanceHatController>();
    public override string Directory => "Appearance/Hat";

    public event Action<AppearanceHatType> OnHatUnlocked;

    public AppearanceHatInfo GetInfo(AppearanceHatType type)
    {
        return Collection.Resources.FirstOrDefault(x => x.Type == type);
    }

    public void Unlock(AppearanceHatType type)
    {
        if (!Data.Game.Appearance.UnlockedHats.Contains(type))
        {
            Data.Game.Appearance.UnlockedHats.Add(type);
        }
    }
}
