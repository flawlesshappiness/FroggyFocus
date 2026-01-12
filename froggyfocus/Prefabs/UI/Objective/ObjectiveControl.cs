using Godot;
using System;
using System.Linq;

public partial class ObjectiveControl : Control
{
    [Export]
    public Label DescriptionLabel;

    [Export]
    public Label ProgressLabel;

    [Export]
    public ProgressBar ProgressBar;

    [Export]
    public Control ProgressBarControl;

    [Export]
    public PriceControl PriceControl;

    [Export]
    public RewardPreview RewardPreview;

    [Export]
    public Button ClaimButton;

    public event Action<ItemType> OnItemUnlocked;

    private ObjectiveInfo info;
    private ObjectiveData data;

    public override void _Ready()
    {
        base._Ready();
        ClaimButton.Pressed += ClaimButton_Pressed;
    }

    public void SetInfo(ObjectiveInfo info)
    {
        this.info = info;
        data = Objective.GetOrCreateData(info);
        DescriptionLabel.Text = info.Description;
        SetLevel(data.Level);
    }

    private void SetLevel(int level)
    {
        var is_max_level = Objective.IsMaxLevel(info);
        var is_max_value = Objective.IsMaxValue(info);

        var max_value = info.Values.ToList().GetClamped(level);
        ProgressBar.Value = data.Value;
        ProgressBar.MaxValue = max_value;
        ProgressLabel.Text = $"{data.Value}/{max_value}";

        var reward_money = GetMoneyReward(level);
        PriceControl.SetPrice(reward_money);
        PriceControl.Visible = !is_max_level && reward_money > 0;

        ClaimButton.Disabled = is_max_level || !is_max_value;
        ClaimButton.Text = is_max_level ? Tr("##COMPLETED##") : is_max_value ? Tr("##CLAIM##") : string.Empty;

        UpdateRewardItem(level);

        ProgressBarControl.Visible = !is_max_level;
        ProgressLabel.Visible = !is_max_value && !is_max_level;
    }

    void UpdateRewardItem(int level)
    {
        if (info.ItemRewards == null || Objective.IsMaxLevel(info))
        {
            RewardPreview.Hide();
            return;
        }

        var reward_item = GetItemReward(level);
        RewardPreview.Visible = !Item.IsNoneType(reward_item);

        var reward_appearance = AppearanceController.Instance.GetInfo(reward_item);
        RewardPreview.SetAppearanceItem(reward_appearance);
    }

    private int GetMoneyReward(int level)
    {
        return info.MoneyRewards.ToList().GetClamped(level);
    }

    private ItemType GetItemReward(int level)
    {
        return info.ItemRewards.ToList().GetClamped(level);
    }

    private void ClaimButton_Pressed()
    {
        var money = GetMoneyReward(data.Level);
        Money.Add(money);

        var item = GetItemReward(data.Level);
        if (!Item.IsNoneType(item))
        {
            Item.MakeOwned(item);
            OnItemUnlocked?.Invoke(item);
        }

        Objective.SetLevel(info, data.Level + 1);
        SetLevel(data.Level);
        Data.Game.Save();
    }
}
