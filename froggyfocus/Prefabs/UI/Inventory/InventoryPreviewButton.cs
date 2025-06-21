using Godot;

public partial class InventoryPreviewButton : ButtonScript
{
    [Export]
    public Node3D Origin;

    [Export]
    public TextureRect TextureRect;

    [Export]
    public SubViewport SubViewport;

    private Node3D current_preview;

    public void SetCharacter(FocusCharacterInfo info)
    {
        var character = info.Scene.Instantiate<FocusCharacter>();
        SetPreview(character);
    }

    public void SetPreview(Node3D node)
    {
        RemovePreview();

        node.SetParent(Origin);
        node.Position = Vector3.Zero;
        node.Rotation = Vector3.Zero;

        TextureRect.Texture = SubViewport.GetTexture();
    }

    private void RemovePreview()
    {
        if (current_preview == null) return;

        current_preview.QueueFree();
        current_preview = null;
    }
}
