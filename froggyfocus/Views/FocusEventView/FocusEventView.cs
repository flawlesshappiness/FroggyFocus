using FlawLizArt.FocusEvent;
using Godot;
using System.Collections;

public partial class FocusEventView : View
{
    public static FocusEventView Instance => Get<FocusEventView>();

    [Export]
    public ProgressBar TimerBar;

    [Export]
    public InputPromptFocus InputPrompt;

    [Export]
    public AnimatedOverlay Overlay_Flash;

    [Export]
    public AnimationPlayer AnimationPlayer_OilSplat;

    private FocusEvent FocusEvent { get; set; }

    public override void _Ready()
    {
        base._Ready();
        InputPrompt.Hide();
    }

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

    public void ShowInputPrompt(string action, Vector3 world_position, InputPromptFocus.AnimationType type)
    {
        var camera = FocusEvent.Camera;
        var position = camera.UnprojectPosition(world_position);
        InputPrompt.GlobalPosition = position;
        InputPrompt.SetAnimation(type);
        InputPrompt.SetInputAction(action);
        InputPrompt.Show();
    }

    public void HideInputPrompt()
    {
        InputPrompt.Hide();
    }

    public void Flash(float duration, Color color)
    {
        this.StartCoroutine(Cr, "flash");
        IEnumerator Cr()
        {
            var overlay = Overlay_Flash.Duplicate() as AnimatedOverlay;
            overlay.SetParent(Overlay_Flash.GetParent());
            yield return overlay.AnimateHide(duration, color);
            overlay.QueueFree();
        }
    }

    public void OilSplat(float duration)
    {
        AnimationPlayer_OilSplat.SpeedScale = 1f / duration;
        AnimationPlayer_OilSplat.Replay("show");
    }
}
