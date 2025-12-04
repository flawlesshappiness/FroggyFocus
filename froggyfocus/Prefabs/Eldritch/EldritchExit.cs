using Godot;

public partial class EldritchExit : Area3D, IInteractable
{
    [Export]
    public string SceneName;

    [Export]
    public string StartNode;

    private bool active_dialogue;

    public override void _Ready()
    {
        base._Ready();
        DialogueController.Instance.OnDialogueEnded += DialogueEnded;
    }

    public void Interact()
    {
        active_dialogue = true;
        DialogueController.Instance.StartDialogue("##ELDRITCH_EXIT_EYE##");
    }

    private void DialogueEnded()
    {
        if (!active_dialogue) return;
        active_dialogue = false;

        GameView.Instance.ShowPopup("##ELDRITCH_EXIT_TEXT##", "##OK##", "##CANCEL##", () =>
        {
            Return();
        },
        () =>
        {

        });
    }

    private void Return()
    {
        EldritchTransitionView.Instance.StartTransitionExit();
    }
}
