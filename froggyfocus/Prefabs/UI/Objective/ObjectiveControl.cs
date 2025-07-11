using Godot;

public partial class ObjectiveControl : Control
{
    [Export]
    public Label DescriptionLabel;

    [Export]
    public Label ProgressLabel;

    [Export]
    public ProgressBar ProgressBar;

    [Export]
    public PriceControl PriceControl;

    [Export]
    public Button ClaimButton;

    [Export]
    public AudioStreamPlayer SfxClaim;

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
        var max_value = info.Values[level];
        ProgressBar.Value = data.Value;
        ProgressBar.MaxValue = max_value;
        ProgressLabel.Text = $"{data.Value}/{max_value}";
        PriceControl.SetPrice(info.MoneyRewards[level]);

        var is_max_level = Objective.IsMaxLevel(info);
        var is_max_value = Objective.IsMaxValue(info);
        ClaimButton.Disabled = is_max_level || !is_max_value;
        ClaimButton.Text = is_max_level ? "MAX" : "CLAIM";
    }

    private void ClaimButton_Pressed()
    {
        var reward = info.MoneyRewards[data.Level];
        Money.Add(reward);

        SfxClaim.Play();

        Objective.SetLevel(info, data.Level + 1);
        SetLevel(data.Level);
        Data.Game.Save();
    }
}
