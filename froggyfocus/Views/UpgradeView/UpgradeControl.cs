using Godot;
using System.Collections.Generic;

public partial class UpgradeControl : Control
{
    [Export]
    public Label NameLabel;

    [Export]
    public Button UpgradeButton;

    [Export]
    public Label UpgradeButtonLabel;

    [Export]
    public Control LevelNodesParent;

    [Export]
    public PackedScene UpgradeLevelNodeTemplate;

    private StatsType type;
    private List<UpgradeLevelNode> level_nodes = new();

    public void Initialize(StatsType type)
    {
        this.type = type;
        var info = StatsController.Instance.GetInfo(type);
        var data = StatsController.Instance.GetOrCreateData(type);
        var levels = UpgradeController.Instance.GetUpgradeMaxLevel(type);
        var upgrade = UpgradeController.Instance.GetInfo(type, data.Level + 1);

        NameLabel.Text = info.Name;

        for (int i = 0; i < levels + 1; i++)
        {
            var node = UpgradeLevelNodeTemplate.Instantiate<UpgradeLevelNode>();
            node.SetParent(LevelNodesParent);
            node.Show();
            level_nodes.Add(node);
        }
    }

    public void Update()
    {
        var data = StatsController.Instance.GetOrCreateData(type);
        var upgrade = UpgradeController.Instance.GetInfo(type, data.Level + 1);
        UpdateButton(upgrade);
        UpdateLevelNodes(data);
    }

    private void UpdateButton(UpgradeInfo upgrade)
    {
        UpgradeButtonLabel.Text = upgrade == null ? $"MAX" : $"{upgrade.Price}";
        UpgradeButton.Disabled = upgrade == null;
    }

    private void UpdateLevelNodes(StatsData data)
    {
        for (int i = 0; i < level_nodes.Count; i++)
        {
            var node = level_nodes[i];
            var active = i <= data.Level;
            node.SetActive(active);
        }
    }
}
