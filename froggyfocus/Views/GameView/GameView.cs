using Godot;
using System;
using System.Collections;

public partial class GameView : View
{
    public static GameView Instance => Get<GameView>();

    [Export]
    public AnimationPlayer AnimationPlayer;

    [Export]
    public ColorRect Overlay;

    [Export]
    public TwoButtonPopup TwoButtonPopup;

    public void AnimateHideOverlay()
    {
        Overlay.Show();
        Overlay.Modulate = Overlay.Modulate.SetA(1);

        this.StartCoroutine(Cr, "transition");
        IEnumerator Cr()
        {
            yield return AnimationPlayer.PlayAndWaitForAnimation("hide_overlay");
        }
    }

    public void ShowPopup(string text, string accept, string cancel, Action on_accept, Action on_cancel)
    {
        TwoButtonPopup.TextLabel.Text = text;
        TwoButtonPopup.AcceptButton.Text = accept;
        TwoButtonPopup.CancelButton.Text = cancel;

        this.StartCoroutine(Cr, nameof(TwoButtonPopup));
        IEnumerator Cr()
        {
            var id = nameof(TwoButtonPopup);
            Player.SetAllLocks(id, true);

            yield return TwoButtonPopup.WaitForPopup();

            if (TwoButtonPopup.Accepted)
            {
                on_accept?.Invoke();
            }
            else
            {
                on_cancel?.Invoke();
            }

            Player.SetAllLocks(id, false);
        }
    }
}
