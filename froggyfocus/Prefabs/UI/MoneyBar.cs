using Godot;

public partial class MoneyBar : Control
{
    [Export]
    public Label MoneyLabel;

    [Export]
    public AnimationPlayer AnimationPlayer;

    public override void _Ready()
    {
        base._Ready();
        Money.OnMoneyChanged += MoneyChanged;
        GameProfileController.Instance.OnGameProfileSelected += OnProfileSelected;
    }

    private void OnProfileSelected(int profile)
    {
        MoneyChanged(0);
    }

    private void MoneyChanged(int amount)
    {
        var value = CurrencyController.Instance.GetValue(CurrencyType.Money);
        MoneyLabel.Text = $"{value}";
    }

    public void AnimateShake()
    {
        AnimationPlayer.Play("shake");
    }
}
