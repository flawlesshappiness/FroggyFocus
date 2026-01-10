using Godot;
using System.Collections;

public partial class MoneyBar : Control
{
    [Export]
    public Label MoneyLabel;

    [Export]
    public Label RewardLabel;

    [Export]
    public AnimationPlayer AnimationPlayer;

    [Export]
    public AudioStreamPlayer SfxValueChanged;

    private int animate_value;
    private Coroutine cr_animate;

    public override void _Ready()
    {
        base._Ready();
        Money.OnMoneyChanged += MoneyChanged;
        GameProfileController.Instance.OnGameProfileSelected += OnProfileSelected;

        RewardLabel.Hide();
    }

    private void OnProfileSelected(int profile)
    {
        UpdateTextImmediatly();
    }

    private void UpdateTextImmediatly()
    {
        var value = CurrencyController.Instance.GetValue(CurrencyType.Money);
        MoneyLabel.Text = $"{value}";
    }

    private void MoneyChanged(int amount)
    {
        Coroutine.Stop(cr_animate);

        if (amount > 0)
        {
            animate_value += amount;
            cr_animate = this.StartCoroutine(Cr, "animate");
        }
        else
        {
            UpdateTextImmediatly();
        }

        SfxValueChanged.Play();

        IEnumerator Cr()
        {
            var current_start = Money.Value - animate_value;
            var current_end = Money.Value;
            var reward_start = animate_value;
            var reward_end = 0;

            RewardLabel.Text = $"+{reward_start}";
            RewardLabel.Show();

            yield return new WaitForSeconds(1f);
            animate_value = 0;

            yield return LerpEnumerator.Lerp01(1f, f =>
            {
                MoneyLabel.Text = $"{(int)Mathf.Lerp(current_start, current_end, f)}";
                RewardLabel.Text = $"{(int)Mathf.Lerp(reward_start, reward_end, f)}";
            });

            RewardLabel.Hide();
        }
    }

    public void AnimateShake()
    {
        AnimationPlayer.Play("shake");
    }
}
