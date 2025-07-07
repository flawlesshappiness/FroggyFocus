using Godot;
using System.Collections;

public partial class PopupControl : ControlScript
{
    [Export]
    public AnimatedOverlay AnimatedOverlay;

    [Export]
    public AnimatedPanel AnimatedPanel;

    [Export]
    public Control InputBlocker;

    [Export]
    public Button FocusButton;

    public bool Active { get; private set; }

    private bool action_performed;

    public override void _Ready()
    {
        base._Ready();
        Hide();
    }

    public IEnumerator WaitForPopup()
    {
        Active = true;
        action_performed = false;

        yield return ShowPopup();
        while (!action_performed) yield return null;
        yield return HidePopup();

        Active = false;
    }

    private IEnumerator ShowPopup()
    {
        Show();
        InputBlocker.Show();
        ReleaseCurrentFocus();

        AnimatedOverlay.AnimateBehindShow();
        yield return AnimatedPanel.AnimatePopShow();

        FocusButton.GrabFocus();

        MouseVisibility.Instance.Lock.AddLock(GetType().Name);

        InputBlocker.Hide();
    }

    private IEnumerator HidePopup()
    {
        InputBlocker.Show();
        ReleaseCurrentFocus();

        MouseVisibility.Instance.Lock.RemoveLock(GetType().Name);

        AnimatedOverlay.AnimateBehindHide();
        yield return AnimatedPanel.AnimatePopHide();

        Hide();
        InputBlocker.Hide();
    }

    protected void ClosePopup()
    {
        action_performed = true;
    }
}
