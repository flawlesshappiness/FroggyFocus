using System.Collections.Generic;

public partial class DemoScene : GameScene
{
    public override void _Ready()
    {
        base._Ready();
        InitializeItems();
        InitializeUpgrades();
    }

    private void InitializeItems()
    {
        var items = new List<ItemType>()
        {
            ItemType.BodyTop_Gradient,
            ItemType.BodyTop_GradientLong,
            ItemType.BodyTop_GradientShort,
            ItemType.BodyPattern_Desert,
            ItemType.BodyEye_Cute,
            ItemType.BodyEye_Derp,
        };

        foreach (var type in items)
        {
            var item = Item.GetOrCreateData(type);
            item.Owned = true;
        }
    }

    private void InitializeUpgrades()
    {
        var speed = UpgradeController.Instance.GetOrCreateData(UpgradeType.CursorSpeed);
        speed.Level = 0;

        var tick = UpgradeController.Instance.GetOrCreateData(UpgradeType.TickAmount);
        tick.Level = 0;

        var time = UpgradeController.Instance.GetOrCreateData(UpgradeType.FocusTime);
        time.Level = 2;
    }
}
