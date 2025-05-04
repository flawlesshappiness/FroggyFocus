using Godot;

public partial class GameView : View
{
    public static GameView Instance => Get<GameView>();

    [Export]
    public Label BugsCollectedLabel;

    public override void _Ready()
    {
        base._Ready();
        FocusEventController.Instance.OnFocusEventCompleted += FocusEventCompleted;
    }

    protected override void OnShow()
    {
        base.OnShow();
        UpdateLabel();
    }

    private void FocusEventCompleted()
    {
        UpdateLabel();
    }

    private void UpdateLabel()
    {
        BugsCollectedLabel.Text = $"Bugs collected: {Data.Game.TargetsCollected}";
    }
}
