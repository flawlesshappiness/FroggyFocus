using Godot;

[GlobalClass]
public partial class AppearanceColorInfo : Resource
{
    [Export]
    public AppearanceColorType Type;

    [Export]
    public string Name;

    [Export]
    public Color Color = Colors.White;

    [Export]
    public int Price;

    [Export]
    public bool Locked;
}
