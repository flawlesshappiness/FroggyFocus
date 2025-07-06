using Godot;

public partial class InventoryPreviewButton : ButtonScript
{
    [Export]
    public Control ValueContainer;

    [Export]
    public Label ValueLabel;

    [Export]
    public TextureRect TextureRect;

    [Export]
    public InventorySubViewport InventorySubViewport;

    public override void _Ready()
    {
        base._Ready();
        TextureRect.Texture = InventorySubViewport.GetTexture();
    }

    public void Clear()
    {
        InventorySubViewport.Clear();
    }

    public void SetCharacter(FocusCharacterInfo info)
    {
        InventorySubViewport.SetCharacter(info);
        ValueLabel.Text = info.CurrencyReward.ToString();
    }

    public void SetObscured(bool obscured)
    {
        var color = obscured ? Colors.Black.SetA(0.5f) : Colors.White;
        TextureRect.Modulate = color;
    }
}
