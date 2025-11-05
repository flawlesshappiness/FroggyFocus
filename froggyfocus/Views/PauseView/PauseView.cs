using Godot;
using System.Collections;

public partial class PauseView : View
{
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
    public Control InputBlocker;

    [Export]
    public OptionsControl Options;

    [Export]
    public CustomizeAppearanceControl CustomizeAppearanceControl;

    [Export]
    public InventoryControl InventoryControl;

    [Export]
    public Button ResumeButton;

    [Export]
    public Button CustomizeButton;

    [Export]
    public Button OptionsButton;

    [Export]
    public Button MainMenuButton;

    [Export]
    public Button InventoryButton;

    public static readonly MultiLock ToggleLock = new();

    private bool animating;
    private bool options_active;
    private bool customize_active;
    private bool inventory_active;
    private bool transitioning;

    public override void _Ready()
    {
        base._Ready();
        ResumeButton.Pressed += ClickResume;
        CustomizeButton.Pressed += ClickCustomize;
        OptionsButton.Pressed += ClickOptions;
        MainMenuButton.Pressed += ClickMainMenu;
        InventoryButton.Pressed += ClickInventory;
        Options.BackPressed += ClickOptionsBack;
        CustomizeAppearanceControl.OnBack += ClickCustomizeBack;
        InventoryControl.OnBack += ClickInventoryBack;
    }

    protected override void OnShow()
    {
        base.OnShow();
        //Scene.PauseLock.AddLock(nameof(PauseView));
        Player.SetAllLocks(nameof(PauseView), true);
        MouseVisibility.Show(nameof(PauseView));
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
        else if (Input.IsActionJustReleased("ui_cancel") && IsVisibleInTree())
        {
            Toggle();
        }
    }

    private void Toggle()
    {
        if (ToggleLock.IsLocked && !Visible) return;
        if (options_active) return;
        if (customize_active) return;
        if (inventory_active) return;
        if (transitioning) return;
        if (animating) return;

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
            yield return AnimatedPanel_Pause.AnimatePopShow();
            InputBlocker.Hide();

            ResumeButton.GrabFocus();

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

    private void ClickCustomize()
    {
        customize_active = true;

        Coroutine.Start(Cr)
            .SetRunWhilePaused();
        IEnumerator Cr()
        {
            animating = true;

            ReleaseCurrentFocus();
            InputBlocker.Show();
            AnimatedPanel_Pause.AnimateShrink();
            yield return AnimatedPanel_Customize.AnimatePopShow();
            InputBlocker.Hide();

            CustomizeAppearanceControl.TabContainer.GetTabBar().GrabFocus();

            animating = false;
        }
    }

    private void ClickCustomizeBack()
    {
        if (animating) return;

        Coroutine.Start(Cr)
            .SetRunWhilePaused();
        IEnumerator Cr()
        {
            animating = true;

            ReleaseCurrentFocus();
            InputBlocker.Show();
            AnimatedPanel_Pause.AnimateGrow();
            yield return AnimatedPanel_Customize.AnimatePopHide();
            InputBlocker.Hide();

            CustomizeButton.GrabFocus();

            customize_active = false;
            animating = false;
        }
    }

    private void ClickOptions()
    {
        options_active = true;

        Coroutine.Start(Cr)
            .SetRunWhilePaused();
        IEnumerator Cr()
        {
            animating = true;

            ReleaseCurrentFocus();
            InputBlocker.Show();
            AnimatedPanel_Pause.AnimateShrink();
            yield return AnimatedPanel_Options.AnimatePopShow();
            InputBlocker.Hide();

            Options.Tabs.GetTabBar().GrabFocus();

            animating = false;
        }
    }

    private void ClickOptionsBack()
    {
        if (animating) return;
        if (!options_active) return;

        Coroutine.Start(Cr)
            .SetRunWhilePaused();
        IEnumerator Cr()
        {
            animating = true;

            ReleaseCurrentFocus();
            InputBlocker.Show();
            AnimatedPanel_Pause.AnimateGrow();
            yield return AnimatedPanel_Options.AnimatePopHide();
            InputBlocker.Hide();

            OptionsButton.GrabFocus();

            options_active = false;
            animating = false;
        }
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
        inventory_active = true;

        Coroutine.Start(Cr)
            .SetRunWhilePaused();
        IEnumerator Cr()
        {
            animating = true;

            ReleaseCurrentFocus();
            InputBlocker.Show();
            AnimatedPanel_Pause.AnimateShrink();
            yield return AnimatedPanel_Inventory.AnimatePopShow();
            InputBlocker.Hide();

            InventoryControl.GrabFocus_InventoryButton();

            animating = false;
        }
    }

    private void ClickInventoryBack()
    {
        if (animating) return;
        if (!inventory_active) return;

        Coroutine.Start(Cr)
            .SetRunWhilePaused();
        IEnumerator Cr()
        {
            animating = true;

            ReleaseCurrentFocus();
            InputBlocker.Show();
            AnimatedPanel_Pause.AnimateGrow();
            yield return AnimatedPanel_Inventory.AnimatePopHide();
            InputBlocker.Hide();

            InventoryButton.GrabFocus();

            inventory_active = false;
            animating = false;
        }
    }
}
