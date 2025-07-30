using Godot;
using System.Collections.Generic;

public partial class UpgradeControl : Control
{
    [Export]
    public Label NameLabel;

    [Export]
    public Label DescriptionLabel;

    [Export]
    public Label MaxLabel;

    [Export]
    public Label CappedLabel;

    [Export]
    public Button UpgradeButton;

    [Export]
    public PriceControl PriceControl;

    [Export]
    public Control LevelNodesParent;

    [Export]
    public PackedScene UpgradeLevelNodeTemplate;

    private UpgradeType type;
    private List<UpgradeLevelNode> level_nodes = new();

    public void Initialize(UpgradeType type)
    {
        this.type = type;
        var info = UpgradeController.Instance.GetInfo(type);
        var data = UpgradeController.Instance.GetOrCreateData(type);
        var levels = UpgradeController.Instance.GetMaxLevel(type);

        NameLabel.Text = info.Name;
        DescriptionLabel.Text = info.Description;

        for (int i = 0; i < levels + 1; i++)
        {
            var node = UpgradeLevelNodeTemplate.Instantiate<UpgradeLevelNode>();
            node.SetParent(LevelNodesParent);
            node.Visible = i <= data.CappedLevel;
            level_nodes.Add(node);
        }

        Visible = data.CappedLevel > 0;
    }

    public void Update()
    {
        var data = UpgradeController.Instance.GetOrCreateData(type);
        var upgrade = UpgradeController.Instance.GetInfo(type);
        UpdateButton(type);
        UpdateLevelNodes(data);
    }

    private void UpdateButton(UpgradeType type)
    {
        var is_max = UpgradeController.Instance.IsMaxLevel(type);
        var is_capped = UpgradeController.Instance.IsCappedLevel(type);
        var price = UpgradeController.Instance.GetCurrentPrice(type);
        PriceControl.SetPrice(price);

        UpgradeButton.Visible = !is_max && !is_capped;
        PriceControl.Visible = !is_max && !is_capped;
        CappedLabel.Visible = is_capped && !is_max;
        MaxLabel.Visible = is_max;
    }

    private void UpdateLevelNodes(UpgradeData data)
    {
        for (int i = 0; i < level_nodes.Count; i++)
        {
            var node = level_nodes[i];
            var active = i <= data.Level;
            node.SetActive(active);
        }
    }
}
