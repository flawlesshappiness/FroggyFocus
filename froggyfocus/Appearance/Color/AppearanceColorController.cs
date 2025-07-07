using Godot;
using System.Linq;

public partial class AppearanceColorController : ResourceController<AppearanceColorCollection, AppearanceColorInfo>
{
    public static AppearanceColorController Instance => Singleton.Get<AppearanceColorController>();
    public override string Directory => "Appearance/Color";

    public AppearanceColorInfo GetInfo(AppearanceColorType type)
    {
        return Collection.Resources.FirstOrDefault(x => x.Type == type);
    }

    public Color GetColor(AppearanceColorType type)
    {
        return GetInfo(type).Color;
    }

    public void Purchase(AppearanceColorType type)
    {
        if (!Data.Game.Appearance.PurchasedColors.Contains(type))
        {
            Data.Game.Appearance.PurchasedColors.Add(type);
        }
    }
}