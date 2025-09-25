using Godot;
using System.Linq;

public partial class AppearanceColorController : ResourceController<AppearanceColorCollection, AppearanceColorInfo>
{
    public static AppearanceColorController Instance => Singleton.Get<AppearanceColorController>();
    public override string Directory => "Appearance/Color";

    public AppearanceColorInfo GetInfo(ItemType type)
    {
        return Collection.Resources.FirstOrDefault(x => x.Type == type);
    }

    public Color GetColor(ItemType type)
    {
        var info = GetInfo(type);

        if (info == null)
        {
            Debug.LogError("Failed to get color of type: " + type);
            return Colors.Pink;
        }

        return info.Color;
    }
}