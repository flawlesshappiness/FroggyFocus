using Godot;
using System.Collections;

public partial class PanelView : View
{
    [Export]
    public AnimatedPanel AnimatedPanel;

    [Export]
    public AnimatedOverlay AnimatedOverlay;

    [Export]
    public Control InputBlocker;

    protected override bool IgnoreCreate => true;
    protected bool Animating { get; set; }

    protected override void OnShow()
    {
        base.OnShow();
        SetLocks(true);
        Open();
    }

    protected override void OnHide()
    {
        base.OnHide();
        SetLocks(false);
    }

    private void SetLocks(bool locked)
    {
        var id = GetType().Name;
        Player.SetAllLocks(id, locked);
        PauseView.ToggleLock.SetLock(id, locked);
        MouseVisibility.SetVisible(id, locked);
    }

    protected virtual void GrabFocusAfterOpen()
    {
        //
    }

    protected void Open()
    {
        if (Animating) return;
        Animating = true;

        this.StartCoroutine(Cr, "animate");
        IEnumerator Cr()
        {
            InputBlocker.Show();
            AnimatedOverlay.AnimateBehindShow();
            yield return AnimatedPanel.AnimatePopShow();
            InputBlocker.Hide();

            GrabFocusAfterOpen();

            Animating = false;
        }
    }

    protected void Close()
    {
        if (Animating) return;
        Animating = true;

        ReleaseCurrentFocus();

        this.StartCoroutine(Cr, "animate");
        IEnumerator Cr()
        {
            InputBlocker.Show();
            AnimatedOverlay.AnimateBehindHide();
            yield return AnimatedPanel.AnimatePopHide();
            InputBlocker.Hide();
            Hide();
            Animating = false;
        }
    }
}
