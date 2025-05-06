using Godot;

public partial class GameView : View
{
    public static GameView Instance => Get<GameView>();

    [Export]
    public Label BugsCollectedLabel;

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

    protected override void OnShow()
    {
        base.OnShow();
        UpdateLabel();
    }

    private void FocusEventStarted()
    {
        FocusBar.Value = 0;
        FocusBar.Show();
    }

    private void FocusEventEnded()
    {
        UpdateLabel();
        FocusBar.Hide();
    }

    private void UpdateLabel()
    {
        BugsCollectedLabel.Text = $"Bugs collected: {Data.Game.TargetsCollected}";
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
}
