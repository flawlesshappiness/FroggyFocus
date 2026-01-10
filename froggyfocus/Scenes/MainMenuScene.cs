using Godot;

public partial class MainMenuScene : GameScene
{
    [Export]
    public Camera3D Camera;

    [Export]
    public FrogCharacter Frog;

    public override void _Ready()
    {
        base._Ready();
        Camera.Current = true;

        GameProfileController.Instance.OnGameProfileSelected += ProfileSelected;
        MainMenuView.Instance.Show();
    }

    protected override void Initialize()
    {
        base.Initialize();
        MainMenuView.Instance.Overlay.AnimateFrontHide();
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        GameProfileController.Instance.OnGameProfileSelected -= ProfileSelected;
    }

    private void ProfileSelected(int profile)
    {
        Frog.LoadAppearance();
    }
}
