using Godot;

public static class Area2DExtensions
{
    public static void SetCollidersEnabled(this Area2D area, bool enabled)
    {
        var colliders = area.GetNodesInChildren<CollisionShape2D>();
        colliders.ForEach(x => x.Disabled = !enabled);
    }
}
