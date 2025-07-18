using Godot;

public partial class PreviewSubViewport : SubViewport
{
    [Export]
    public Node3D Origin;

    private Node3D current_preview;

    public void Clear()
    {
        if (current_preview == null) return;

        current_preview.QueueFree();
        current_preview = null;
    }

    public void SetPreview(Node3D preview)
    {
        Clear();

        if (preview == null) return;

        current_preview = preview;
        current_preview.SetParent(Origin);
        current_preview.Position = Vector3.Zero;
        current_preview.Rotation = Vector3.Zero;
    }

    public Node3D SetPrefab(PackedScene prefab) => SetPrefab<Node3D>(prefab);
    public T SetPrefab<T>(PackedScene prefab)
        where T : Node3D
    {
        if (prefab == null) return null;

        var preview = prefab.Instantiate<Node3D>();
        SetPreview(preview);
        return preview as T;
    }
}
