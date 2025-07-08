using Godot;

public partial class MainMenuScene : Scene
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
