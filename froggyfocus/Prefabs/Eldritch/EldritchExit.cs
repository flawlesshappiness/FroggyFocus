using Godot;

public partial class EldritchExit : Area3D, IInteractable
{
    [Export]
    public string SceneName;

    [Export]
    public string StartNode;

    private const string ExitId = "##ELDRITCH_EXIT_EYE##";

    public override void _Ready()
    {
        base._Ready();
        DialogueController.Instance.OnDialogueEnded += DialogueEnded;
    }

    public void Interact()
    {
        DialogueController.Instance.StartDialogue(ExitId);
    }

    private void DialogueEnded(string id)
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
        EldritchTransitionView.Instance.StartTransitionExit();
    }
}
