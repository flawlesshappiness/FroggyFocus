using Godot;

public partial class GameView : View
{
    public static GameView Instance => Get<GameView>();

    [Export]
    public Label BugsCollectedLabel;

    [Export]
    public ProgressBar FocusBar;

    public override void _Ready()
    {
        base._Ready();
        FocusEventController.Instance.OnFocusEventStarted += FocusEventStarted;
        FocusEventController.Instance.OnFocusEventCompleted += FocusEventEnded;
        FocusEventController.Instance.OnFocusEventFailed += FocusEventEnded;
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

    public void FocusValueChanged(float value)
    {
        FocusBar.Value = value;
    }
}
