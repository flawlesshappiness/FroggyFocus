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

    [Export]
    public ProgressBar ShieldBar;

    [Export]
    public AnimationPlayer AnimationPlayer_Quests;

    [Export]
    public AnimationPlayer AnimationPlayer_Money;

    [Export]
    public AnimationPlayer AnimationPlayer_Vignette;

    [Export]
    public InputPromptControl InputPrompt;

    private FocusEvent current_focus_event;
    private bool skip_quest_advanced;

    private Coroutine cr_money;
    private float time_money_show;

    public override void _Ready()
    {
        base._Ready();

        FocusEventController.Instance.OnFocusEventStarted += FocusEventStarted;
        FocusEventController.Instance.OnFocusEventCompleted += _ => FocusEventEnded();
        FocusEventController.Instance.OnFocusEventFailed += _ => FocusEventEnded();
        MainQuestController.Instance.OnAnyQuestAdvanced += AnyQuestAdvanced;
        Money.OnMoneyChanged += MoneyChanged;

        SetFocusEventControlsVisible(false);
    }

    protected override void Initialize()
    {
        base.Initialize();
        PauseView.Instance.OnViewShow += PauseViewShow;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        Process_ShieldBar();
    }

    private void Process_ShieldBar()
    {
        if (current_focus_event == null) return;

        ShieldBar.Value = current_focus_event.Cursor.ShieldPercentage;
    }

    public void SetFocusEventControlsVisible(bool visible)
    {
        ShieldBar.Visible = visible;
    }

    private void FocusEventStarted(FocusEvent e)
    {
        current_focus_event = e;
    }

    private void FocusEventEnded()
    {
        current_focus_event = null;
    }

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

    private void PauseViewShow()
    {
        skip_quest_advanced = true;
    }

    public void TriggerQuestAdvancedNotification()
    {
        AnyQuestAdvanced();
    }

    private void AnyQuestAdvanced()
    {
        skip_quest_advanced = false;
        this.StartCoroutine(Cr, "quest_advanced");

        IEnumerator Cr()
        {
            yield return AnimationPlayer_Quests.PlayAndWaitForAnimation("show");

            var time_end = GameTime.Time + 5f;
            while (GameTime.Time < time_end && !skip_quest_advanced)
            {
                yield return null;
            }

            yield return AnimationPlayer_Quests.PlayAndWaitForAnimation("hide");
        }
    }

    private void MoneyChanged(int value)
    {
        time_money_show = GameTime.Time + 5f;
        if (cr_money != null) return;

        cr_money = this.StartCoroutine(Cr, "show");
        IEnumerator Cr()
        {
            yield return AnimationPlayer_Money.PlayAndWaitForAnimation("show");
            while (GameTime.Time < time_money_show) yield return null;
            cr_money = null;
            yield return AnimationPlayer_Money.PlayAndWaitForAnimation("hide");
        }
    }

    public Coroutine AnimateVignetteShow(float duration)
    {
        AnimationPlayer_Vignette.SpeedScale = 1f / duration;
        return AnimationPlayer_Vignette.PlayAndWaitForAnimation("show");
    }

    public Coroutine AnimateVignetteHide(float duration)
    {
        AnimationPlayer_Vignette.SpeedScale = 1f / duration;
        return AnimationPlayer_Vignette.PlayAndWaitForAnimation("hide");
    }
}
