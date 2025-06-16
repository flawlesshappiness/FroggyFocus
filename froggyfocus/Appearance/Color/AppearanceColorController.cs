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
}