using Godot;
using Godot.Collections;

[GlobalClass]
public partial class FocusEventInfo : Resource
{
    [Export]
    public FocusEventAxis Axis;

    [Export]
    public Array<FocusCharacterInfo> Characters;
}
