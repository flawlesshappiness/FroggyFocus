using Godot;
using System;
using System.Collections;

public partial class TutorialPopup : ControlScript
{
    [Export]
    public AnimatedOverlay Overlay;

    [Export]
    public AnimatedPanel Panel;

    [Export]
    public Control InputBlocker;

    [Export]
    public Button ContinueButton;

    public event Action OnContinue;

    private bool animating;

    public override void _Ready()
    {
        base._Ready();
        Hide();
        InputBlocker.Hide();
        ContinueButton.Pressed += Continue_Pressed;
    }

    protected override void OnShow()
    {
        base.OnShow();
        Scene.PauseLock.SetLock(nameof(TutorialPopup), true);
        MouseVisibility.Show(nameof(TutorialPopup));
    }

    protected override void OnHide()
    {
        base.OnHide();
        Scene.PauseLock.SetLock(nameof(TutorialPopup), false);
        MouseVisibility.Hide(nameof(TutorialPopup));
    }

    public Coroutine ShowPopup()
    {
        Show();
        return this.StartCoroutine(Cr, "popup")
            .SetRunWhilePaused();
        IEnumerator Cr()
        {
            animating = true;
            this.ReleaseCurrentFocus();
            InputBlocker.Show();
            ContinueButton.Disabled = true;

            Overlay.AnimateBehindShow();
            yield return Panel.AnimatePopShow();

            yield return new WaitForSeconds(1f, true);

            ContinueButton.Disabled = false;
            ContinueButton.GrabFocus();

            InputBlocker.Hide();
            animating = false;
        }
    }

    public Coroutine HidePopup()
    {
        return this.StartCoroutine(Cr, "popup")
            .SetRunWhilePaused();
        IEnumerator Cr()
        {
            animating = true;
            this.ReleaseCurrentFocus();
            InputBlocker.Show();

            Overlay.AnimateBehindHide();
            yield return Panel.AnimatePopHide();

            InputBlocker.Hide();
            animating = false;

            OnContinue?.Invoke();
            Hide();
        }
    }

    private void Continue_Pressed()
    {
        if (animating) return;

        HidePopup();
    }
}
