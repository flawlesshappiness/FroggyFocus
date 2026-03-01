using FlawLizArt.FocusEvent;
using Godot;

public partial class FocusEventView : View
{
    public static FocusEventView Instance => Get<FocusEventView>();

    [Export]
    public ProgressBar TimerBar;

    private FocusEvent FocusEvent { get; set; }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        Process_TimerBar();
    }

    public void SetFocusEvent(FocusEvent focus_event)
    {
        FocusEvent = focus_event;
    }

    private void Process_TimerBar()
    {
        if (FocusEvent == null) return;

        if (FocusEvent.IsStarting)
        {
            TimerBar.Value = TimerBar.MaxValue;
        }
        else if (FocusEvent.IsEnding)
        {
            TimerBar.Value = TimerBar.MinValue;
        }
        else if (FocusEvent.IsRunning)
        {
            var max = FocusEvent.TimerDuration;
            var value = (GameTime.Time - FocusEvent.TimerStart);
            var t = Mathf.Clamp(value / max, 0, 1);

            TimerBar.Value = 1.0f - t;
        }
    }
}
