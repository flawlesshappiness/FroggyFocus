using Godot;

[GlobalClass]
public partial class HandInRequestInfo : Resource
{
    [Export]
    public int Count;

    [Export]
    public FocusCharacterTag TargetTag;

    [Export]
    public FocusCharacterInfo TargetInfo;

    [Export]
    public int Money;

    public bool HasMoney => Money > 0;

    public InventoryFilterOptions GetInventoryFilterOptions()
    {
        var filter = new InventoryFilterOptions();

        if (TargetInfo != null)
        {
            filter.ValidCharacters = new() { TargetInfo };
        }
        else
        {
            filter.ValidTags = new() { TargetTag };
        }

        return filter;
    }

    public string GetRequestText()
    {
        if (TargetInfo != null)
        {
            return $"Catch {Count} {TargetInfo.Name}";
        }
        else
        {
            return $"Catch {Count} {GetTagText(TargetTag)}";
        }
    }

    public string GetTagText(FocusCharacterTag tag) => tag switch
    {
        FocusCharacterTag.Wooden => "wooden bugs",
        FocusCharacterTag.Weeded => "weeded bugs",
        FocusCharacterTag.Sandy => "sandy bugs",
        FocusCharacterTag.Oily => "oily bugs",
        FocusCharacterTag.Mossy => "mossy bugs",
        FocusCharacterTag.Infested => "infested bugs",
        FocusCharacterTag.Flower => "blooming bugs",
        FocusCharacterTag.Polluted => "polluted bugs",
        FocusCharacterTag.Crystalized => "crystallized bugs",
        FocusCharacterTag.Water => "swimming bugs",
        FocusCharacterTag.Flying => "winged bugs",
        _ => "bugs"
    };
}
