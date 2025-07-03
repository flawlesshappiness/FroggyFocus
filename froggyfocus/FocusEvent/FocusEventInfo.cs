using Godot;
using Godot.Collections;

[GlobalClass]
public partial class FocusEventInfo : Resource
{
    [Export]
    public FocusCharacterInfo OverrideCharacter;

    [Export]
    public Array<FocusCharacterInfo> Characters;

    public FocusCharacterInfo GetRandomCharacter()
    {
        return OverrideCharacter ?? Characters.PickRandom();
    }
}
