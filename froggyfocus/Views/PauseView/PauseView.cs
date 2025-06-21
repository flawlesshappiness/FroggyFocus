using Godot;
using System.Collections;

public partial class PauseView : View
{
    [Export]
    public AnimationPlayer AnimationPlayer_Pause;

    [Export]
    public AnimationPlayer AnimationPlayer_Options;

    [Export]
    public AnimationPlayer AnimationPlayer_Customize;

    [Export]
    public AnimationPlayer AnimationPlayer_Inventory;

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

    [Export]
    public ColorRect Overlay;

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
        Options.OnBack += ClickOptionsBack;
        CustomizeAppearanceControl.OnBack += ClickCustomizeBack;
        InventoryControl.OnBack += ClickInventoryBack;
    }

    protected override void OnShow()
    {
        base.OnShow();
        Scene.PauseLock.AddLock(nameof(PauseView));
        MouseVisibility.Show(nameof(PauseView));
    }

    protected override void OnHide()
    {
        base.OnHide();
        Scene.PauseLock.RemoveLock(nameof(PauseView));
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
        if (UpgradeView.Instance.Visible) return;
        if (MainMenuView.Instance.Visible) return;
        if (options_active) return;
        if (customize_active) return;
        if (inventory_active) return;
        if (transitioning) return;

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
            InputBlocker.Show();
            yield return AnimationPlayer_Pause.PlayAndWaitForAnimation("show");
            InputBlocker.Hide();
            transitioning = false;

            ResumeButton.GrabFocus();
        }
    }

    private Coroutine Close()
    {
        transitioning = true;

        return this.StartCoroutine(Cr, "transition")
            .SetRunWhilePaused();
        IEnumerator Cr()
        {
            InputBlocker.Show();
            yield return AnimationPlayer_Pause.PlayAndWaitForAnimation("hide");
            InputBlocker.Hide();

            transitioning = false;
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
            InputBlocker.Show();
            AnimationPlayer_Pause.Play("to_background");
            yield return AnimationPlayer_Customize.PlayAndWaitForAnimation("show");
            InputBlocker.Hide();

            CustomizeAppearanceControl.TabContainer.GetTabBar().GrabFocus();
        }
    }

    private void ClickCustomizeBack()
    {
        Coroutine.Start(Cr)
            .SetRunWhilePaused();
        IEnumerator Cr()
        {
            InputBlocker.Show();
            AnimationPlayer_Pause.Play("to_foreground");
            yield return AnimationPlayer_Customize.PlayAndWaitForAnimation("hide");
            InputBlocker.Hide();

            CustomizeButton.GrabFocus();

            customize_active = false;
        }
    }

    private void ClickOptions()
    {
        options_active = true;

        Coroutine.Start(Cr)
            .SetRunWhilePaused();
        IEnumerator Cr()
        {
            InputBlocker.Show();
            AnimationPlayer_Pause.Play("to_background");
            yield return AnimationPlayer_Options.PlayAndWaitForAnimation("show");
            InputBlocker.Hide();

            Options.Tabs.GetTabBar().GrabFocus();
        }
    }

    private void ClickOptionsBack()
    {
        if (!options_active) return;

        Coroutine.Start(Cr)
            .SetRunWhilePaused();
        IEnumerator Cr()
        {
            InputBlocker.Show();
            AnimationPlayer_Pause.Play("to_foreground");
            yield return AnimationPlayer_Options.PlayAndWaitForAnimation("hide");
            InputBlocker.Hide();

            OptionsButton.GrabFocus();

            options_active = false;
        }
    }

    private void ClickMainMenu()
    {
        Coroutine.Start(Cr)
            .SetRunWhilePaused();
        IEnumerator Cr()
        {
            transitioning = true;

            InputBlocker.Show();
            yield return AnimationPlayer_Pause.PlayAndWaitForAnimation("hide");
            yield return AnimationPlayer_Pause.PlayAndWaitForAnimation("show_overlay");
            Scene.Goto<MainMenuScene>();

            Overlay.Hide();
            Hide();
            InputBlocker.Hide();

            transitioning = false;
        }
    }

    private void ClickInventory()
    {
        inventory_active = true;

        Coroutine.Start(Cr)
            .SetRunWhilePaused();
        IEnumerator Cr()
        {
            InputBlocker.Show();
            AnimationPlayer_Pause.Play("to_background");
            yield return AnimationPlayer_Inventory.PlayAndWaitForAnimation("show");
            InputBlocker.Hide();

            InventoryControl.GrabFocus_InventoryButton();
        }
    }

    private void ClickInventoryBack()
    {
        if (!inventory_active) return;

        Coroutine.Start(Cr)
            .SetRunWhilePaused();
        IEnumerator Cr()
        {
            InputBlocker.Show();
            AnimationPlayer_Pause.Play("to_foreground");
            yield return AnimationPlayer_Inventory.PlayAndWaitForAnimation("hide");
            InputBlocker.Hide();

            InventoryButton.GrabFocus();

            inventory_active = false;
        }
    }
}
