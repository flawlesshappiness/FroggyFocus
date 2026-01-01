using Godot;
using System.Collections;

public partial class HandInView : View
{
    public static HandInView Instance => Get<HandInView>();

    [Export]
    public AnimatedOverlay AnimatedOverlay;

    [Export]
    public AnimatedPanel AnimatedPanel_HandIn;

    [Export]
    public HandInContainer HandInContainer;

    [Export]
    public UnlockPopup UnlockPopup;

    [Export]
    public Control InputBlocker;

    private bool animating;

    public override void _Ready()
    {
        base._Ready();
        HandInContainer.BackButton.Pressed += BackButton_Pressed;
        HandInContainer.PinButton.Pressed += PinButton_Pressed;
        HandInContainer.OnClaim += HandInContainer_Claim;
    }

    protected override void OnShow()
    {
        base.OnShow();
        SetLocks(true);
    }

    protected override void OnHide()
    {
        base.OnHide();
        SetLocks(false);
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (Input.IsActionJustReleased("ui_cancel") && IsVisibleInTree() && !animating)
        {
            Close();
        }
    }

    public void ShowPopup(string id)
    {
        var data = HandIn.GetOrCreateData(id);
        ShowPopup(data);
    }

    public void ShowPopup(HandInData data)
    {
        if (data == null) return;

        HandInContainer.Load(data);
        Show();

        StartCoroutine(Cr, "animate");
        IEnumerator Cr()
        {
            animating = true;

            InputBlocker.Show();
            AnimatedOverlay.AnimateBehindShow();
            yield return AnimatedPanel_HandIn.AnimatePopShow();
            HandInContainer.GetFirstButton().GrabFocus();
            InputBlocker.Hide();

            animating = false;
        }
    }

    private void SetLocks(bool locked)
    {
        var id = nameof(HandInView);
        Player.SetAllLocks(id, locked);
        MouseVisibility.Instance.Lock.SetLock(id, locked);
    }

    private void BackButton_Pressed()
    {
        Close();
    }

    private void PinButton_Pressed()
    {
        Close();
    }

    private void HandInContainer_Claim()
    {
        Close();
    }

    private Coroutine Close()
    {
        return StartCoroutine(Cr, "animate");
        IEnumerator Cr()
        {
            animating = true;

            ReleaseCurrentFocus();
            InputBlocker.Show();
            yield return HandInContainer.WaitForRewardBarFill();
            AnimatedOverlay.AnimateBehindHide();
            yield return AnimatedPanel_HandIn.AnimatePopHide();
            InputBlocker.Hide();
            yield return WaitForPopup();
            Hide();

            animating = false;

            if (!HandInContainer.CurrentClaimed)
            {
                HandInController.Instance.HandInClosed(HandInContainer.CurrentInfo);
            }
        }
    }

    private bool CanShowPopup()
    {
        return HandInContainer.HasItemUnlock &&
            HandInContainer.IsMaxClaim &&
            HandInContainer.RewardUnlockBar.Visible;
    }

    private IEnumerator WaitForPopup()
    {
        if (HandInContainer.CurrentClaimed)
        {
            if (CanShowPopup())
            {
                UnlockPopup.SetItemUnlock();
                UnlockPopup.SetAppearanceItem(HandInContainer.CurrentInfo.ItemUnlock);
                yield return UnlockPopup.WaitForPopup();
            }

            HandInController.Instance.HandInClaimed(HandInContainer.CurrentData);
        }
    }
}
