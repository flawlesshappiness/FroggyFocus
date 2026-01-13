using Godot;
using System;
using System.Collections;

public partial class MainMenuView : View
{
    public static MainMenuView Instance => Get<MainMenuView>();

    [Export]
    public MainMenuContainer MainMenuContainer;

    [Export]
    public OptionsControl OptionsControl;

    [Export]
    public SaveProfilesContainer SaveProfilesContainer;

    [Export]
    public CreditsContainer CreditsContainer;

    [Export]
    public Control InputBlocker;

    [Export]
    public AnimatedOverlay Overlay;

    [Export]
    public AnimatedPanel AnimatedPanel_Main;

    [Export]
    public AnimatedPanel AnimatedPanel_Options;

    [Export]
    public AnimatedPanel AnimatedPanel_Profiles;

    [Export]
    public AnimatedPanel AnimatedPanel_Credits;

    private bool animating;
    private MenuSettings current_menu;

    public event Action OnMainMenuEnter;
    public event Action OnGameStart;

    private class MenuSettings
    {
        public AnimatedPanel Panel { get; set; }
        public Func<Control> GetFocusControl { get; set; }
        public Func<Control> GetBackFocusControl { get; set; }
    }

    public override void _Ready()
    {
        base._Ready();
        MainMenuContainer.NewGameButton.Pressed += ClickContinue;
        MainMenuContainer.ContinueButton.Pressed += ClickContinue;
        MainMenuContainer.ProfilesButton.Pressed += ClickProfiles;
        MainMenuContainer.OptionsButton.Pressed += ClickOptions;
        MainMenuContainer.CreditsButton.Pressed += ClickCredits;
        MainMenuContainer.QuitButton.Pressed += ClickQuit;
        OptionsControl.BackPressed += CloseMenu;
        SaveProfilesContainer.ProfilePressed += CloseMenu;
        CreditsContainer.OnBackPressed += CloseMenu;
        GameProfileController.Instance.OnGameProfileSelected += GameProfileSelected;

        Overlay.AnimateShowImmediate();

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
        UpdateContinueButton();

        GameView.Instance.Hide();

        OnMainMenuEnter?.Invoke();

        Open();
    }

    protected override void Initialize()
    {
        base.Initialize();
        SaveProfilesContainer.LoadProfiles();
    }

    private void GameProfileSelected(int profile)
    {
        UpdateContinueButton();
    }

    private void UpdateContinueButton()
    {
        MainMenuContainer.NewGameButton.Visible = Data.Game.Deleted;
        MainMenuContainer.ContinueButton.Visible = !Data.Game.Deleted;
    }

    private void Open()
    {
        if (animating) return;
        animating = true;

        Overlay.AnimateShowImmediate();

        this.StartCoroutine(Cr, "transition");
        IEnumerator Cr()
        {
            yield return AnimatedPanel_Main.AnimateFadeShow();

            var button = Data.Game.Deleted ? MainMenuContainer.NewGameButton : MainMenuContainer.ContinueButton;
            button.GrabFocus();
            animating = false;
        }
    }

    private void ShowMenu(MenuSettings settings)
    {
        if (animating) return;
        if (current_menu != null) return;
        current_menu = settings;

        Coroutine.Start(Cr)
            .SetRunWhilePaused();
        IEnumerator Cr()
        {
            animating = true;

            ReleaseCurrentFocus();
            InputBlocker.Show();
            yield return AnimatedPanel_Main.AnimateFadeHide();
            yield return settings.Panel.AnimateMoveUp();
            InputBlocker.Hide();

            settings.GetFocusControl().GrabFocus();

            animating = false;
        }
    }

    private void CloseMenu()
    {
        if (animating) return;
        if (current_menu == null) return;
        var settings = current_menu;
        current_menu = null;

        Coroutine.Start(Cr)
            .SetRunWhilePaused();
        IEnumerator Cr()
        {
            animating = true;

            ReleaseCurrentFocus();
            InputBlocker.Show();
            yield return settings.Panel.AnimateMoveDown();
            yield return AnimatedPanel_Main.AnimateFadeShow();
            InputBlocker.Hide();

            settings.GetBackFocusControl().GrabFocus();

            animating = false;
        }
    }

    private void ClickContinue()
    {
        if (animating) return;
        animating = true;

        Data.Game.Deleted = false;
        Data.Game.Save();

        this.StartCoroutine(Cr, "transition");
        IEnumerator Cr()
        {
            ReleaseCurrentFocus();
            InputBlocker.Show();

            yield return AnimatedPanel_Main.AnimateFadeHide();
            yield return Overlay.AnimateFrontShow();

            Hide();
            PauseView.ToggleLock.SetLock(nameof(MainMenuView), false);
            MouseVisibility.Hide(nameof(MainMenuView));
            Scene.Goto(Data.Game.CurrentScene);
            GameView.Instance.Show();
            GameView.Instance.AnimateHideOverlay();

            OnGameStart?.Invoke();

            InputBlocker.Hide();
            animating = false;
        }
    }

    private void ClickProfiles()
    {
        ShowMenu(new MenuSettings
        {
            Panel = AnimatedPanel_Profiles,
            GetFocusControl = () => SaveProfilesContainer.ProfileControl1.ProfileButton,
            GetBackFocusControl = () => MainMenuContainer.ProfilesButton
        });
    }

    private void ClickOptions()
    {
        ShowMenu(new MenuSettings
        {
            Panel = AnimatedPanel_Options,
            GetFocusControl = () => OptionsControl.Tabs.GetTabBar(),
            GetBackFocusControl = () => MainMenuContainer.OptionsButton
        });
    }

    private void ClickCredits()
    {
        ShowMenu(new MenuSettings
        {
            Panel = AnimatedPanel_Credits,
            GetFocusControl = () => CreditsContainer.BackButton,
            GetBackFocusControl = () => MainMenuContainer.CreditsButton
        });
    }

    private void ClickQuit()
    {
        ReleaseCurrentFocus();
        Scene.Tree.Quit();
    }
}
