using Godot;

public partial class MainMenuView : View
{
    public override string Directory => $"{Paths.ViewDirectory}";
    public static MainMenuView Instance => Get<MainMenuView>();

    [Export]
    public Button ContinueButton;

    [Export]
    public Button OptionsButton;

    [Export]
    public Button QuitButton;

    public override void _Ready()
    {
        base._Ready();
        ContinueButton.Pressed += ClickContinue;
        OptionsButton.Pressed += ClickOptions;
        QuitButton.Pressed += ClickQuit;
    }

    protected override void OnShow()
    {
        base.OnShow();
        MouseVisibility.Show(nameof(MainMenuView));
    }

    private void ClickContinue()
    {
        Hide();
        Scene.Goto<SwampScene>();
    }

    private void ClickOptions()
    {
        Hide();
        OptionsView.Instance.BackView = this;
        OptionsView.Instance.Show();
    }

    private void ClickQuit()
    {
        Scene.Tree.Quit();
    }
}
