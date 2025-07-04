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
        var category = "Inventory";

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
}
