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

    public InventoryCharacterData CreateCharacterData(FocusCharacterInfo info)
    {
        var data = new InventoryCharacterData();
        var rng = new RandomNumberGenerator();

        // Info
        data.InfoPath = info.ResourcePath;

        // Size
        data.Size = info.OverrideSize > 0 ? info.OverrideSize : rng.RandfRange(0.4f, 0.6f);

        // Stars
        data.Stars = GetRarity(info, Player.Instance.HasHotspot);

        // Value
        data.Value = GetMoneyValue(info, data);

        return data;
    }

    private int GetRarity(FocusCharacterInfo info, bool is_hotspot)
    {
        var wrng = new WeightedRandom<int>();
        if (info.OverrideRarity > 0)
        {
            return info.OverrideRarity;
        }
        else if (Player.Instance.MaxRarity > 0)
        {
            for (int i = 1; i <= Player.Instance.MaxRarity; i++)
            {
                wrng.AddElement(i, 1f / i);
            }
        }
        else if (is_hotspot)
        {
            wrng.AddElement(1, 5);
            wrng.AddElement(2, 4);
            wrng.AddElement(3, 3);
            wrng.AddElement(4, 2);
            wrng.AddElement(5, 1);
        }
        else
        {
            wrng.AddElement(1, 0);
            wrng.AddElement(2, 4);
            wrng.AddElement(3, 3);
            wrng.AddElement(4, 2);
            wrng.AddElement(5, 1);
        }

        return wrng.Random();
    }

    private int GetMoneyValue(FocusCharacterInfo info, InventoryCharacterData data)
    {
        var base_value = Constants.BUG_BASE_VALUE;
        var base_multiplier = info.MoneyMultiplier;

        var stars_value = 5 * data.Stars;
        //var stars_multiplier = 1f + Mathf.Clamp(-0.2f + 0.1f * data.Stars, 0f, 1f);
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

    public int GetCurrentSize()
    {
        return (int)UpgradeController.Instance.GetCurrentValue(UpgradeType.InventorySize);
    }

    public bool IsInventoryFull()
    {
        return Data.Game.Inventory.Characters.Count >= GetCurrentSize();
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

        if (options.ExcludedDatas?.Contains(data) ?? false) return false;
        if (!options.ValidCharacters?.Contains(info) ?? false) return false;
        if (options.ExcludedCharacters?.Contains(info) ?? false) return false;

        return true;
    }
}

public class InventoryFilterOptions
{
    public List<FocusCharacterInfo> ValidCharacters { get; set; }
    public List<FocusCharacterInfo> ExcludedCharacters { get; set; }
    public List<InventoryCharacterData> ExcludedDatas { get; set; }
}
