using Godot;

public partial class FocusEventTutorialView : View
{
    public static FocusEventTutorialView Instance => Get<FocusEventTutorialView>();

    [Export]
    public TutorialPopup CatchTutorial;

    public const string CatchTutorialFlag = "catch_tutorial_flag";

    public override void _Ready()
    {
        base._Ready();
        CatchTutorial.OnContinue += Popup_Continue;
    }

    public void StartCatchTutorial()
    {
        if (GameFlags.IsFlag(CatchTutorialFlag, 1) && !Data.Options.CatchTutorialEnabled) return;


        Data.Options.CatchTutorialEnabled = false;
        GameFlags.SetFlag(CatchTutorialFlag, 1);

        Show();
        CatchTutorial.ShowPopup();
    }

    private void Popup_Continue()
    {
        Hide();
    }
}
