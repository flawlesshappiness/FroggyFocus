using Godot;

public partial class MainMenuScene : GameScene
{
    [Export]
    public Camera3D Camera;

    public override void _Ready()
    {
        base._Ready();
        MainMenuView.Instance.Show();

        Camera.Current = true;
    }
}
