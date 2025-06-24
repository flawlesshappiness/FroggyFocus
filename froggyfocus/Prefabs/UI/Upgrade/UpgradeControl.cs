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
        var levels = UpgradeController.Instance.GetMaxLevel(type);

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
        var upgrade = UpgradeController.Instance.GetInfo(type);
        UpdateButton(type);
        UpdateLevelNodes(data);
    }

    private void UpdateButton(StatsType type)
    {
        var is_max = UpgradeController.Instance.IsMaxLevel(type);
        var price = UpgradeController.Instance.GetCurrentPrice(type);
        UpgradeButtonLabel.Text = is_max ? $"MAX" : $"{price}";
        UpgradeButton.Disabled = is_max;
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
