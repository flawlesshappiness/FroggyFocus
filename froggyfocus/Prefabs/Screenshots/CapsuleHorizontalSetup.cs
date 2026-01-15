using Godot;
using Godot.Collections;

public partial class CapsuleHorizontalSetup : ScreenshotSceneSetup
{
    [Export]
    public Array<FocusCharacter> Bugs;

    public override void _Ready()
    {
        base._Ready();

        foreach (var bug in Bugs)
        {
            bug.SetAccessory(null);
        }
    }
}
