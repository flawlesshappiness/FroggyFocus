using Godot;
using System.Collections;

public partial class PauseView : View
{
    [Export]
    public AnimationPlayer AnimationPlayer;

    [Export]
    public Control InputBlocker;

    [Export]
    public OptionsControl Options;

    [Export]
    public Button ResumeButton;

    [Export]
    public Button OptionsButton;

    [Export]
    public Button MainMenuButton;

    [Export]
    public ColorRect Overlay;

    private bool options_active;

    public override void _Ready()
    {
        base._Ready();
        ResumeButton.Pressed += ClickResume;
        OptionsButton.Pressed += ClickOptions;
        MainMenuButton.Pressed += ClickMainMenu;
        Options.OnBack += ClickOptionsBack;
    }

    protected override void OnShow()
    {
        base.OnShow();
        Scene.PauseLock.AddLock(nameof(PauseView));
    }

    protected override void OnHide()
    {
        base.OnHide();
        Scene.PauseLock.RemoveLock(nameof(PauseView));
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
        if (UpgradeView.Instance.Visible) return;
        if (MainMenuView.Instance.Visible) return;
        if (options_active) return;

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

        return this.StartCoroutine(Cr, "transition")
            .SetRunWhilePaused();
        IEnumerator Cr()
        {
            InputBlocker.Show();
            yield return AnimationPlayer.PlayAndWaitForAnimation("show");
            InputBlocker.Hide();
        }
    }

    private Coroutine Close()
    {
        return this.StartCoroutine(Cr, "transition")
            .SetRunWhilePaused();
        IEnumerator Cr()
        {
            InputBlocker.Show();
            yield return AnimationPlayer.PlayAndWaitForAnimation("hide");
            InputBlocker.Hide();

            Hide();
        }
    }

    private void ClickResume()
    {
        Close();
    }

    private void ClickOptions()
    {
        options_active = true;

        Coroutine.Start(Cr)
            .SetRunWhilePaused();
        IEnumerator Cr()
        {
            InputBlocker.Show();
            yield return AnimationPlayer.PlayAndWaitForAnimation("show_options");
            InputBlocker.Hide();
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
            yield return AnimationPlayer.PlayAndWaitForAnimation("hide_options");
            InputBlocker.Hide();

            options_active = false;
        }
    }

    private void ClickMainMenu()
    {
        Coroutine.Start(Cr)
            .SetRunWhilePaused();
        IEnumerator Cr()
        {
            InputBlocker.Show();
            yield return AnimationPlayer.PlayAndWaitForAnimation("hide");
            yield return AnimationPlayer.PlayAndWaitForAnimation("show_overlay");
            Scene.Goto<MainMenuScene>();

            Overlay.Hide();
            Hide();
            InputBlocker.Hide();
        }
    }
}
