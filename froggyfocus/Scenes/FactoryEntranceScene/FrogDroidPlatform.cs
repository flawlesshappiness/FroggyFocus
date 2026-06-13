using Godot;

public partial class FrogDroidPlatform : Node3D
{
    [Export]
    public CuteFrogCharacter Character;

    public override void _Ready()
    {
        base._Ready();
        InitializeAppearance();
    }

    private void InitializeAppearance()
    {
        Character.ClearAppearance();
        Character.SetBodyBase(Colors.Gray);
        Character.SetBodyTop(ItemType.BodyEye_Robot, Colors.Black);
        Character.SetBodyEye(ItemType.BodyEye_Robot, Colors.Red);
    }
}
