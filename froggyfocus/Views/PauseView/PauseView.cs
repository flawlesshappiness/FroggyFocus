using Godot;
using System;
using System.Collections;

public partial class PauseView : View
{
    public static PauseView Instance => Get<PauseView>();

    [Export]
    public AnimatedOverlay AnimatedOverlay_Behind;

    [Export]
    public AnimatedOverlay AnimatedOverlay_Front;

    [Export]
    public AnimatedPanel AnimatedPanel_Pause;

    [Export]
    public AnimatedPanel AnimatedPanel_Options;

    [Export]
    public AnimatedPanel AnimatedPanel_Customize;

    [Export]
    public AnimatedPanel AnimatedPanel_Inventory;

    [Export]
    public AnimatedPanel AnimatedPanel_Bestiary;

    [Export]
    public Control InputBlocker;

    [Export]
    public OptionsControl Options;

    [Export]
    public CustomizeAppearanceControl CustomizeAppearanceControl;

    [Export]
    public InventoryControl InventoryControl;

    [Export]
    public BestiaryControl BestiaryControl;

    [Export]
    public PauseContainer PauseContainer;

    [Export]
    public PinsContainer PinsContainer;

    [Export]
    public AnimationPlayer AnimationPlayer_Pins;

    [Export]
    public AnimationPlayer AnimationPlayer_Quests;

    [Export]
    public AnimationPlayer AnimationPlayer_Money;

    public static readonly MultiLock ToggleLock = new();

    public event Action OnViewShow;

    private bool animating;
    private bool transitioning;
    private MenuSettings current_menu;

    private class MenuSettings
    {
        public AnimatedPanel MenuPanel { get; set; }
        public Func<Control> GetFocusControl { get; set; }
        public Func<Control> GetBackFocusControl { get; set; }
    }

    public override void _Ready()
    {
        base._Ready();
        PauseContainer.ResumeButton.Pressed += ClickResume;
        PauseContainer.CustomizeButton.Pressed += ClickCustomize;
        PauseContainer.OptionsButton.Pressed += ClickOptions;
        PauseContainer.MainMenuButton.Pressed += ClickMainMenu;
        PauseContainer.InventoryButton.Pressed += ClickInventory;
        PauseContainer.BestiaryButton.Pressed += ClickBestiary;
        Options.BackPressed += CloseMenu;
        CustomizeAppearanceControl.OnBack += CloseMenu;
        InventoryControl.OnBack += CloseMenu;
        BestiaryControl.OnBack += CloseMenu;
        PinsContainer.OnPinsEmpty += PinsEmpty;
    }

    protected override void OnShow()
    {
        base.OnShow();
        //Scene.PauseLock.AddLock(nameof(PauseView));
        Player.SetAllLocks(nameof(PauseView), true);
        MouseVisibility.Show(nameof(PauseView));

        OnViewShow?.Invoke();
    }

    protected override void OnHide()
    {
        base.OnHide();
        //Scene.PauseLock.RemoveLock(nameof(PauseView));
        Player.SetAllLocks(nameof(PauseView), false);
        MouseVisibility.Hide(nameof(PauseView));
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (PlayerInput.Pause.Pressed)
        {
            Toggle();
        }
    }

    private void Toggle()
    {
        if (ToggleLock.IsLocked && !Visible) return;
        if (transitioning) return;
        if (animating) return;
        if (current_menu != null) return;

        if (Visible)
        {
            Close();
        }
        else
        {
            Open();
        }
    }

    private Coroutine Open()
    {
        Show();
        transitioning = true;

        return this.StartCoroutine(Cr, "transition")
            .SetRunWhilePaused();
        IEnumerator Cr()
        {
            animating = true;

            InputBlocker.Show();
            AnimatedOverlay_Behind.AnimateBehindShow();
            ShowPins();
            ShowQuests();
            ShowMoney();
            yield return AnimatedPanel_Pause.AnimatePopShow();
            InputBlocker.Hide();

            PauseContainer.ResumeButton.GrabFocus();

            transitioning = false;
            animating = false;
        }
    }

    private Coroutine Close()
    {
        transitioning = true;

        return this.StartCoroutine(Cr, "transition")
            .SetRunWhilePaused();
        IEnumerator Cr()
        {
            animating = true;

            ReleaseCurrentFocus();
            InputBlocker.Show();
            AnimatedOverlay_Behind.AnimateBehindHide();
            HidePins();
            HideQuests();
            HideMoney();
            yield return AnimatedPanel_Pause.AnimatePopHide();
            InputBlocker.Hide();

            transitioning = false;
            animating = false;
            Hide();
        }
    }

    private void ClickResume()
    {
        Close();
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
            AnimatedPanel_Pause.AnimateShrink();
            HidePins();
            HideQuests();
            yield return settings.MenuPanel.AnimatePopShow();
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
            AnimatedPanel_Pause.AnimateGrow();
            ShowPins();
            ShowQuests();
            yield return settings.MenuPanel.AnimatePopHide();
            InputBlocker.Hide();

            settings.GetBackFocusControl().GrabFocus();

            animating = false;
        }
    }

    private void ClickCustomize()
    {
        ShowMenu(new MenuSettings
        {
            MenuPanel = AnimatedPanel_Customize,
            GetFocusControl = () => CustomizeAppearanceControl.TabContainer.GetTabBar(),
            GetBackFocusControl = () => PauseContainer.CustomizeButton
        });
    }

    private void ClickOptions()
    {
        ShowMenu(new MenuSettings
        {
            MenuPanel = AnimatedPanel_Options,
            GetFocusControl = () => Options.Tabs.GetTabBar(),
            GetBackFocusControl = () => PauseContainer.OptionsButton
        });
    }

    private void ClickMainMenu()
    {
        if (animating) return;

        Coroutine.Start(Cr)
            .SetRunWhilePaused();
        IEnumerator Cr()
        {
            animating = true;
            transitioning = true;

            ReleaseCurrentFocus();
            InputBlocker.Show();
            AnimatedOverlay_Behind.AnimateBehindHide();
            yield return AnimatedPanel_Pause.AnimatePopHide();
            yield return AnimatedOverlay_Front.AnimateFrontShow();

            Data.Game.Save();

            WeatherController.Instance.StopWeather();
            WorldBugController.Instance.Stop();
            FocusHotSpotController.Instance.Stop();
            Scene.Goto<MainMenuScene>();

            Hide();
            AnimatedOverlay_Front.Hide();
            InputBlocker.Hide();

            transitioning = false;
            animating = false;
        }
    }

    private void ClickInventory()
    {
        ShowMenu(new MenuSettings
        {
            MenuPanel = AnimatedPanel_Inventory,
            GetFocusControl = () => InventoryControl.GetFocusControl(),
            GetBackFocusControl = () => PauseContainer.InventoryButton
        });
    }

    private void ClickBestiary()
    {
        ShowMenu(new MenuSettings
        {
            MenuPanel = AnimatedPanel_Bestiary,
            GetFocusControl = () => BestiaryControl.GetFocusControl(),
            GetBackFocusControl = () => PauseContainer.BestiaryButton
        });
    }

    private void ShowPins()
    {
        AnimationPlayer_Pins.Play("show");
    }

    private void HidePins()
    {
        AnimationPlayer_Pins.Play("hide");
    }

    private void PinsEmpty()
    {
        PauseContainer.MainMenuButton.GrabFocus();
    }

    private void ShowQuests()
    {
        AnimationPlayer_Quests.Play("show");
    }

    private void HideQuests()
    {
        AnimationPlayer_Quests.Play("hide");
    }

    private void ShowMoney()
    {
        AnimationPlayer_Money.Play("show");
    }

    private void HideMoney()
    {
        AnimationPlayer_Money.Play("hide");
    }
}
