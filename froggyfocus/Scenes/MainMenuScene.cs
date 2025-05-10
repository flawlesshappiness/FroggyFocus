public partial class MainMenuScene : Scene
{
    public override void _Ready()
    {
        base._Ready();
        MainMenuView.Instance.Show();
    }
}
