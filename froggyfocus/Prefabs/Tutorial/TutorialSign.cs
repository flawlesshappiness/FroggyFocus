using Godot;

public partial class TutorialSign : Area3D, IInteractable
{
    [Export]
    public string DialogueNode;

    [Export]
    public Type SignType;

    public enum Type
    {
        Jump,
        Bugs,
        Shield
    }

    public void Interact()
    {
        //DialogueController.Instance.StartDialogue(DialogueNode);
        TutorialView.Instance.ShowSign(SignType);
    }
}
