using System.Collections.Generic;
using System.Linq;

public partial class InventoryController : SingletonController
{
    public static InventoryController Instance => Singleton.Get<InventoryController>();
    public override string Directory => "Inventory";

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
            Action = DebugAddCharacterData
        });

        void DebugAddCharacterData(DebugView v)
        {
            v.HideContent();
            v.SetContent_Search();

            foreach (var info in FocusCharacterController.Instance.Collection.Resources)
            {
                v.ContentSearch.AddItem(info.Name, () => AddCharacter(info));
            }

            v.ContentSearch.UpdateButtons();
        }
    }

    public void AddCharacter(FocusCharacterInfo info)
    {
        var data = new InventoryCharacterData
        {
            InfoPath = info.ResourcePath,
        };

        Data.Game.Inventory.Characters.Add(data);
        Data.Game.Save();
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
