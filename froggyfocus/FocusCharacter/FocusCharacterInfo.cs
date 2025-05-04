using Godot;

[GlobalClass]
public partial class FocusCharacterInfo : Resource
{
    [Export]
    public PackedScene Scene;

    [Export]
    public Vector2 SizeRange = new Vector2(1, 1);

    [Export]
    public float MoveSpeed = 1f;

    [Export]
    public Vector2 MoveLengthRange = new Vector2(1, 1);

    [Export]
    public Vector2 MoveDelayRange = new Vector2(1, 1);

    [Export]
    public FocusEventAxis MoveAxis = FocusEventAxis.XZ;
}
