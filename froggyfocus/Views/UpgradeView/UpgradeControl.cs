using Godot;

public partial class UpgradeControl : Control
{
    [Export]
    public Label NameLabel;

    [Export]
    public Button UpgradeButton;

    [Export]
    public Control LevelNodesParent;

    [Export]
    public PackedScene UpgradeLevelNodeTemplate;

    public void SetUpgrade(StatsType type)
    {
        var info = StatsController.Instance.GetInfo(type);
        var data = StatsController.Instance.GetOrCreateData(type);
        var levels = UpgradeController.Instance.GetUpgradeMaxLevel(type);
        var upgrade = UpgradeController.Instance.GetInfo(type, data.Level + 1);

        NameLabel.Text = info.Name;

        UpgradeButton.Text = upgrade == null ? $"MAX" : $"{upgrade.Price}";
        UpgradeButton.Disabled = upgrade == null;

        for (int i = 0; i < levels; i++)
        {
            var node = UpgradeLevelNodeTemplate.Instantiate<UpgradeLevelNode>();
            node.SetParent(LevelNodesParent);
            node.Show();

            var active = i <= data.Level;
            node.SetActive(active);
        }
    }
}
