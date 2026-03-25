using System.Collections.Generic;

public partial class DemoScene : GameScene
{
    public override void _Ready()
    {
        base._Ready();
        InitializeItems();
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
}
