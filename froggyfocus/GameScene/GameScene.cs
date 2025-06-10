using Godot;

public partial class GameScene : Scene
{
    [Export]
    public FocusEvent FocusEvent; // Will probably have multiple later on, like for water or land

    public static GameScene Instance { get; private set; }

    public override void _Ready()
    {
        base._Ready();
        Instance = this;
    }
}
