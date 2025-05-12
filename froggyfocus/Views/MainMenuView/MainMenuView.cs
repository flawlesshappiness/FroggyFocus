using Godot;
using System.Collections;

public partial class MainMenuView : View
{
    public static MainMenuView Instance => Get<MainMenuView>();

    [Export]
    public AnimationPlayer AnimationPlayer;

    [Export]
    public OptionsControl OptionsControl;

    [Export]
    public Button ContinueButton;

    [Export]
    public Button OptionsButton;

    [Export]
    public Button QuitButton;

    [Export]
    public Control InputBlocker;

    public override void _Ready()
    {
        base._Ready();
        ContinueButton.Pressed += ClickContinue;
        OptionsButton.Pressed += ClickOptions;
        QuitButton.Pressed += ClickQuit;
        OptionsControl.OnBack += ClickOptionsBack;

        InputBlocker.Hide();
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
        GameView.Instance.Show();
    }

    private void ClickOptions()
    {
        this.StartCoroutine(Cr, "transition");
        IEnumerator Cr()
        {
            InputBlocker.Show();
            yield return AnimationPlayer.PlayAndWaitForAnimation("hide_main");
            yield return AnimationPlayer.PlayAndWaitForAnimation("show_options");
            InputBlocker.Hide();
        }
    }

    private void ClickOptionsBack()
    {
        this.StartCoroutine(Cr, "transition");
        IEnumerator Cr()
        {
            InputBlocker.Show();
            yield return AnimationPlayer.PlayAndWaitForAnimation("hide_options");
            yield return AnimationPlayer.PlayAndWaitForAnimation("show_main");
            InputBlocker.Hide();
        }
    }

    private void ClickQuit()
    {
        Scene.Tree.Quit();
    }
}
