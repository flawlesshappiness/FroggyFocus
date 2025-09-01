using Godot;

public partial class FactoryEntryDoor : Area3D, IInteractable
{
    public void Interact()
    {
        DialogueController.Instance.StartDialogue("##LOCKED##");
    }
}
