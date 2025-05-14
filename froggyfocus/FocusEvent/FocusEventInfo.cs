using Godot;
using Godot.Collections;

[GlobalClass]
public partial class FocusEventInfo : Resource
{
    [Export]
    public Array<FocusCharacterInfo> Characters;
}
