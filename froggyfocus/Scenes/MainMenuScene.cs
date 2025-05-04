public partial class MainMenuScene : Scene
{
    public override void _Ready()
    {
        base._Ready();
        GameProfileController.Instance.SelectGameProfile(Data.Options.SelectedGameProfile);
        MainMenuView.Instance.Show();
    }
}
