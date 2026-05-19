using Godot;

public static class Node2DExtensions
{
    public static Vector2 RightDirection(this Node2D node) => node.GlobalTransform.BasisXform(Vector2.Right);
    public static Vector2 LeftDirection(this Node2D node) => -node.RightDirection();
    public static Vector2 UpDirection(this Node2D node) => node.GlobalTransform.BasisXform(Vector2.Up);
    public static Vector2 DownDirection(this Node2D node) => -node.UpDirection();
}
