using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class InventoryController : SingletonController
{
    public static InventoryController Instance => Singleton.Get<InventoryController>();
    public override string Directory => "Inventory";

    public event Action<InventoryCharacterData> OnCharacterAdded;

    protected override void Initialize()
    {
        base.Initialize();
        RegisterDebugActions();
    }

    private void RegisterDebugActions()
    {
        var category = "INVENTORY";

        Debug.RegisterAction(new DebugAction
        {
            Category = category,
            Text = "Add CharacterData",
            Action = SelectCharacter
        });

        void SelectCharacter(DebugView v)
        {
            v.HideContent();
            v.SetContent_Search();

            foreach (var info in FocusCharacterController.Instance.Collection.Resources)
            {
                v.ContentSearch.AddItem(info.Name, () => DebugAddCharacterData(info));
            }

            v.ContentSearch.UpdateButtons();
        }

        void DebugAddCharacterData(FocusCharacterInfo info)
        {
            var data = CreateCharacterData(info);
            AddCharacter(data);
        }
    }

    public InventoryCharacterData CreateCharacterData(FocusCharacterInfo info, int? rarity_override = null, int? rarity_max = null)
    {
        var data = new InventoryCharacterData();
        var rng = new RandomNumberGenerator();

        // Info
        data.InfoPath = info.ResourcePath;

        // Stars
        data.Stars = rarity_override ?? GetRarity(info, max: rarity_max ?? 0);
        var t_stars = Mathf.Clamp((data.Stars - 1) / 4f, 0, 1);

        // Size
        data.Size = info.OverrideSize > 0 ? info.OverrideSize : Mathf.Lerp(0.6f, 1.0f, t_stars);

        // Value
        data.Value = GetMoneyValue(info, data);

        return data;
    }

    private int GetRarity(FocusCharacterInfo info, int max = 0)
    {
        var wrng = new WeightedRandom<int>();
        if (info.OverrideRarity > 0)
        {
            return info.OverrideRarity;
        }
        else
        {
            AddRarity(1, 1);
            AddRarity(2, 4);
            AddRarity(3, 3);
            AddRarity(4, 2);
            AddRarity(5, 1);
        }

        return wrng.Random();

        void AddRarity(int i, float w)
        {
            if (max > 0 && i > max) return;
            wrng.AddElement(i, w);
        }
    }

    private int GetMoneyValue(FocusCharacterInfo info, InventoryCharacterData data)
    {
        var base_value = Constants.BUG_BASE_VALUE;
        var base_multiplier = info.MoneyMultiplier;

        var stars_value = 5 * data.Stars;
        var stars_mul_min = 1f;
        var stars_mul_max = 2f;
        var t_stars = (data.Stars - 1) / 4f;
        var stars_mul = Mathf.Lerp(stars_mul_min, stars_mul_max, Curves.EaseInQuad.Evaluate(t_stars));

        var size_value = (int)(20 * data.Size);
        var tag_multiplier = GetTagMoneyMultiplier(info);
        var multiplier = base_multiplier * stars_mul * tag_multiplier;

        var value = (int)((base_value + stars_value + size_value) * multiplier);
        return value;
    }

    private float GetTagMoneyMultiplier(FocusCharacterInfo info)
    {
        var significant_tags = new List<FocusCharacterTag> {
            FocusCharacterTag.Sandy, FocusCharacterTag.Crystalized, FocusCharacterTag.Mossy,
            FocusCharacterTag.Infested, FocusCharacterTag.Flower, FocusCharacterTag.Wooden,
            FocusCharacterTag.Oily, FocusCharacterTag.Weeded, FocusCharacterTag.Polluted };

        var has_significant = info.Tags.Intersect(significant_tags).Any();

        if (has_significant)
        {
            return 1.25f;
        }
        else
        {
            return 1.0f;
        }
    }

    public void AddCharacter(InventoryCharacterData data)
    {
        Data.Game.Inventory.Characters.Add(data);
        Data.Game.Save();

        OnCharacterAdded?.Invoke(data);
    }

    public void RemoveCharacterData(InventoryCharacterData data)
    {
        Data.Game.Inventory.Characters.Remove(data);
    }

    public List<InventoryCharacterData> GetCharactersInInventory(InventoryFilterOptions options = null)
    {
        return Data.Game.Inventory.Characters
            .Where(x => IsDataValid(x, options))
            .ToList();
    }

    public bool HasCharacter(FocusCharacterInfo info)
    {
        return Data.Game.Inventory.Characters.Any(x => x.InfoPath == info.ResourcePath);
    }

    public bool IsDataValid(InventoryCharacterData data, InventoryFilterOptions options = null)
    {
        if (options == null) return true;

        var info = FocusCharacterController.Instance.GetInfoFromPath(data.InfoPath);

        if (options.ValidTags?.Intersect(info.Tags).Count() == 0) return false;
        if (options.ExcludedDatas?.Contains(data) ?? false) return false;
        if (!ValidCharacters()) return false;
        if (options.ExcludedCharacters?.Contains(info) ?? false) return false;

        return true;

        bool ValidCharacters()
        {
            if (options.ValidCharacters == null) return true;
            if (options.ValidCharacters.Count == 0) return true;

            foreach (var v in options.ValidCharacters)
            {
                if (v.Name == v.Variation) // Is base version
                {
                    if (info.Variation == v.Variation)
                        return true; // Base or variation match
                }
                else // Is variation
                {
                    if (info == v)
                        return true; // Variation match
                }
            }

            return false;
        }
    }
}

public class InventoryFilterOptions
{
    public List<FocusCharacterTag> ValidTags { get; set; }
    public List<FocusCharacterInfo> ValidCharacters { get; set; }
    public List<FocusCharacterInfo> ExcludedCharacters { get; set; }
    public List<InventoryCharacterData> ExcludedDatas { get; set; }
}
