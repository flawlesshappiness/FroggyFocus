using Godot;

[GlobalClass]
public partial class LocationInfo : Resource
{
    [Export]
    public string Id;

    [Export]
    public string Scene;

    [Export]
    public string Name;

    [Export]
    public int Price;

    [Export]
    public Texture2D PreviewImage;
}
