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

    [Export]
    public ColorRect Overlay;

    private bool animating;

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
        PauseView.ToggleLock.SetLock(nameof(MainMenuView), true);
        MouseVisibility.Show(nameof(MainMenuView));

        GameView.Instance.Hide();

        Open();
    }

    private void Open()
    {
        if (animating) return;
        animating = true;

        Overlay.Show();
        Overlay.Modulate = Overlay.Modulate.SetA(1);

        this.StartCoroutine(Cr, "transition");
        IEnumerator Cr()
        {
            yield return AnimationPlayer.PlayAndWaitForAnimation("hide_overlay");

            ContinueButton.GrabFocus();
            animating = false;
        }
    }

    private void ClickContinue()
    {
        if (animating) return;
        animating = true;

        this.StartCoroutine(Cr, "transition");
        IEnumerator Cr()
        {
            InputBlocker.Show();
            yield return AnimationPlayer.PlayAndWaitForAnimation("show_overlay");

            Hide();
            PauseView.ToggleLock.SetLock(nameof(MainMenuView), false);
            MouseVisibility.Hide(nameof(MainMenuView));
            Scene.Goto<SwampScene>();
            GameView.Instance.Show();
            GameView.Instance.AnimateHideOverlay();

            InputBlocker.Hide();
            animating = false;
        }
    }

    private void ClickOptions()
    {
        if (animating) return;
        animating = true;

        this.StartCoroutine(Cr, "transition");
        IEnumerator Cr()
        {
            InputBlocker.Show();
            yield return AnimationPlayer.PlayAndWaitForAnimation("hide_main");
            yield return AnimationPlayer.PlayAndWaitForAnimation("show_options");
            InputBlocker.Hide();

            OptionsControl.Tabs.GetTabBar().GrabFocus();
            animating = false;
        }
    }

    private void ClickOptionsBack()
    {
        if (animating) return;
        animating = true;

        this.StartCoroutine(Cr, "transition");
        IEnumerator Cr()
        {
            InputBlocker.Show();
            yield return AnimationPlayer.PlayAndWaitForAnimation("hide_options");
            yield return AnimationPlayer.PlayAndWaitForAnimation("show_main");
            InputBlocker.Hide();

            OptionsButton.GrabFocus();
            animating = false;
        }
    }

    private void ClickQuit()
    {
        Scene.Tree.Quit();
    }
}
