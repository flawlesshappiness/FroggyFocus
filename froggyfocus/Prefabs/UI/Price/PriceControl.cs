using Godot;

public partial class PriceControl : Control
{
    [Export]
    public Label PriceLabel;

    public void SetPrice(int price)
    {
        PriceLabel.Text = price.ToString();
    }
}
