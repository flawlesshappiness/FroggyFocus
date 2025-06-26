using Godot;

public partial class RewardPreview : Control
{
    [Export]
    public ObjectPreview ObjectPreview;

    [Export]
    public Label AmountLabel;

    [Export]
    public Node3D CoinStack;

    public void Clear()
    {
        CoinStack.Hide();
    }

    public void SetCoinStack()
    {
        Clear();
        CoinStack.Show();
    }

    public void SetAmount(int amount)
    {
        AmountLabel.Text = $"x{amount}";
    }
}
