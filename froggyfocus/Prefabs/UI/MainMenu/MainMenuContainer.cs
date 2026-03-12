using Godot;

public partial class MainMenuContainer : MarginContainer
{
    [Export]
    public Button PlayDemoButton;

    [Export]
    public Button NewGameButton;

    [Export]
    public Button ContinueButton;

    [Export]
    public Button ProfilesButton;

    [Export]
    public Button OptionsButton;

    [Export]
    public Button CreditsButton;

    [Export]
    public Button QuitButton;

    [Export]
    public Label VersionLabel;

    [Export]
    public Label DemoLabel;

    public override void _Ready()
    {
        base._Ready();
        VersionLabel.Text = ApplicationInfo.Instance.GetVersionString();
        DemoLabel.Visible = ApplicationInfo.Instance.Type == ApplicationType.Demo;
    }
}
