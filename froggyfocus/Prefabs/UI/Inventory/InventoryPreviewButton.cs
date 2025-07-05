using Godot;

public partial class InventoryPreviewButton : ButtonScript
{
    [Export]
    public Control ValueContainer;

    [Export]
    public Label ValueLabel;

    [Export]
    public Node3D Origin;

    [Export]
    public TextureRect TextureRect;

    [Export]
    public SubViewport SubViewport;

    private Node3D current_preview;

    public void Clear()
    {
        RemovePreview();
    }

    public void SetCharacter(FocusCharacterInfo info)
    {
        var character = info.Scene.Instantiate<FocusCharacter>();
        character.Initialize(info);
        SetPreview(character);

        ValueLabel.Text = info.CurrencyReward.ToString();
    }

    public void SetPreview(Node3D node)
    {
        RemovePreview();

        current_preview = node;
        current_preview.SetParent(Origin);
        current_preview.Position = Vector3.Zero;
        current_preview.Rotation = Vector3.Zero;

        TextureRect.Texture = SubViewport.GetTexture();
    }

    private void RemovePreview()
    {
        if (current_preview == null) return;

        current_preview.QueueFree();
        current_preview = null;
    }

    public void SetObscured(bool obscured)
    {
        var color = obscured ? Colors.Black.SetA(0.5f) : Colors.White;
        TextureRect.Modulate = color;
    }
}
