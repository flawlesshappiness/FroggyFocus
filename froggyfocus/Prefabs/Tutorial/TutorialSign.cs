using Godot;

public partial class TutorialSign : Area3D, IInteractable
{
    [Export]
    public string DialogueNode;

    public void Interact()
    {
        DialogueController.Instance.StartDialogue(DialogueNode);
    }
}
