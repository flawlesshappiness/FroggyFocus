using Godot;

public partial class EldritchExit : Area3D, IInteractable
{
    [Export]
    public string SceneName;

    [Export]
    public string StartNode;

    private const string ExitId = "ELDRITCH_EXIT_EYE";

    public override void _Ready()
    {
        base._Ready();
        DialogueController.Instance.OnEntryEnded += DialogueEntry_Ended;
    }

    public void Interact()
    {
        DialogueController.Instance.StartDialogue(ExitId);
    }

    private void DialogueEntry_Ended(string id)
    {
        if (id == ExitId)
        {
            GameView.Instance.ShowPopup("##ELDRITCH_EXIT_TEXT##", "##OK##", "##CANCEL##", () =>
            {
                Return();
            },
            () =>
            {

            });
        }
    }

    private void Return()
    {
        Data.Game.CurrentScene = nameof(SwampScene);
        Data.Game.StartingNode = "EldritchStart";
        Data.Game.Save();

        TransitionView.Instance.StartTransition(new TransitionSettings
        {
            Type = TransitionType.Color,
            Color = Colors.Black,
            Duration = 0.5f,
            OnTransition = OnTransition
        });

        void OnTransition()
        {
            EldritchTransitionView.Instance.StartTransitionExit();
        }
    }
}
