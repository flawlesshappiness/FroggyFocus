using Godot;

public partial class FocusTarget : Node3D
{
    public float Size { get; set; }
    public float Radius => Size * 0.5f;

    public void SetSize(float size)
    {
        Size = size;
        Scale = Vector3.One * size;
    }
}
