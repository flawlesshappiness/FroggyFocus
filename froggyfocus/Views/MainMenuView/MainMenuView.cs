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
    public SaveProfilesContainer SaveProfilesContainer;

    [Export]
    public Button ContinueButton;

    [Export]
    public Button ProfilesButton;

    [Export]
    public Button OptionsButton;

    [Export]
    public Button QuitButton;

    [Export]
    public Control InputBlocker;

    [Export]
    public ColorRect Overlay;

    [Export]
    public Label VersionLabel;

    private bool animating;

    public override void _Ready()
    {
        base._Ready();
        ContinueButton.Pressed += ClickContinue;
        ProfilesButton.Pressed += ClickProfiles;
        OptionsButton.Pressed += ClickOptions;
        QuitButton.Pressed += ClickQuit;
        OptionsControl.BackPressed += ClickOptionsBack;
        SaveProfilesContainer.ProfilePressed += ClickProfilesBack;

        VersionLabel.Text = ApplicationInfo.Instance.GetVersionString();

        InputBlocker.Hide();

        Show();
        OnShow();
    }

    protected override void OnShow()
    {
        base.OnShow();
        PauseView.ToggleLock.SetLock(nameof(MainMenuView), true);
        MouseVisibility.Show(nameof(MainMenuView));

        SaveProfilesContainer.LoadProfiles();

        GameView.Instance.Hide();

        Open();
    }

    protected override void Initialize()
    {
        base.Initialize();
        SaveProfilesContainer.LoadProfiles();
    }

    public void AnimateHideOverlay()
    {
        AnimationPlayer.Play("hide_overlay");
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
            ReleaseCurrentFocus();
            InputBlocker.Show();
            yield return AnimationPlayer.PlayAndWaitForAnimation("show_overlay");

            Hide();
            PauseView.ToggleLock.SetLock(nameof(MainMenuView), false);
            MouseVisibility.Hide(nameof(MainMenuView));
            Scene.Goto(Data.Game.CurrentScene);
            GameView.Instance.Show();
            GameView.Instance.AnimateHideOverlay();

            InputBlocker.Hide();
            animating = false;
        }
    }

    private void ClickProfiles()
    {
        if (animating) return;
        animating = true;

        this.StartCoroutine(Cr, "transition");
        IEnumerator Cr()
        {
            ReleaseCurrentFocus();
            InputBlocker.Show();
            yield return AnimationPlayer.PlayAndWaitForAnimation("hide_main");
            yield return AnimationPlayer.PlayAndWaitForAnimation("show_profiles");
            InputBlocker.Hide();

            SaveProfilesContainer.ProfileControl1.ProfileButton.GrabFocus();
            animating = false;
        }
    }

    private void ClickProfilesBack()
    {
        if (animating) return;
        animating = true;

        this.StartCoroutine(Cr, "transition");
        IEnumerator Cr()
        {
            ReleaseCurrentFocus();
            InputBlocker.Show();
            yield return AnimationPlayer.PlayAndWaitForAnimation("hide_profiles");
            yield return AnimationPlayer.PlayAndWaitForAnimation("show_main");
            InputBlocker.Hide();

            ProfilesButton.GrabFocus();
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
            ReleaseCurrentFocus();
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
            ReleaseCurrentFocus();
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
        ReleaseCurrentFocus();
        Scene.Tree.Quit();
    }
}
