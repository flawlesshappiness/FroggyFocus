using Godot;

public partial class EldritchReturn : Area3D, IInteractable
{
    [Export]
    public Node3D ReturnNode;

    private bool active_dialogue;

    public override void _Ready()
    {
        base._Ready();
        DialogueController.Instance.OnDialogueEnded += DialogueEnded;
    }

    public void Interact()
    {
        active_dialogue = true;
        DialogueController.Instance.StartDialogue("##ELDRITCH_RETURN_EYE##");
    }

    private void DialogueEnded()
    {
        if (!active_dialogue) return;
        active_dialogue = false;

        GameView.Instance.ShowPopup("##ELDRITCH_RETURN_TEXT##", "##OK##", "##CANCEL##", () =>
        {
            Return();
        },
        () =>
        {

        });
    }

    private void Return()
    {
        EldritchTransitionView.Instance.StartTransitionShort(() =>
        {
            Player.Instance.GlobalPosition = ReturnNode.GlobalPosition;
        });
    }
}
