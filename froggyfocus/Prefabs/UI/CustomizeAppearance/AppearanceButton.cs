using Godot;

public partial class AppearanceButton : ButtonScript
{
    [Export]
    public Node3D Origin;

    [Export]
    public TextureRect TextureRect;

    [Export]
    public SubViewport SubViewport;

    public void SetPrefab(PackedScene prefab)
    {
        if (prefab == null) return;

        var preview = prefab.Instantiate<Node3D>();
        preview.SetParent(Origin);
        preview.Position = Vector3.Zero;
        preview.Rotation = Vector3.Zero;
        preview.Scale = Vector3.One;

        TextureRect.Texture = SubViewport.GetTexture();
    }
}
