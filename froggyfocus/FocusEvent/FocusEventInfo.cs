using Godot;
using Godot.Collections;

[GlobalClass]
public partial class FocusEventInfo : Resource
{
    [Export]
    public string Id;

    [Export]
    public Vector2I TargetCount;

    [Export]
    public Array<FocusCharacterInfo> Characters;
}
