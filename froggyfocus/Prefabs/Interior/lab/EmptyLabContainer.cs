using Godot;

public partial class EmptyLabContainer : Node3DScript
{
    [Export]
    public InteractArea InteractArea;

    private const string DialogueInteract = "LAB_CONTAINER";

    public override void _Ready()
    {
        base._Ready();
        InteractArea.OnInteract += Interact;
    }

    private void Interact()
    {
        DialogueController.Instance.StartDialogue(DialogueInteract);
    }
}
