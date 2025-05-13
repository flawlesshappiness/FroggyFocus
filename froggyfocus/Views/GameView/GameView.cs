using Godot;
using System.Collections;

public partial class GameView : View
{
    public static GameView Instance => Get<GameView>();

    [Export]
    public AnimationPlayer AnimationPlayer;

    [Export]
    public ColorRect Overlay;

    [Export]
    public ProgressBar FocusBar;

    [Export]
    public AudioStreamPlayer SfxFocusGain;

    public override void _Ready()
    {
        base._Ready();
        FocusEventController.Instance.OnFocusEventStarted += FocusEventStarted;
        FocusEventController.Instance.OnFocusEventCompleted += FocusEventEnded;
        FocusEventController.Instance.OnFocusEventFailed += FocusEventEnded;

        FocusBar.Hide();
    }

    private void FocusEventStarted()
    {
        FocusBar.Value = 0;
        FocusBar.Show();
    }

    private void FocusEventEnded(FocusEventResult result)
    {
        FocusBar.Hide();
    }

    public void FocusGained(float value)
    {
        FocusValueChanged(value);

        var pitch_min = 0.5f;
        var pitch_max = 1.5f;
        SfxFocusGain.PitchScale = Mathf.Lerp(pitch_min, pitch_max, value);
        SfxFocusGain.Play();
    }

    public void FocusLost(float value)
    {
        FocusValueChanged(value);
    }

    private void FocusValueChanged(float value)
    {
        FocusBar.Value = value;
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
}
