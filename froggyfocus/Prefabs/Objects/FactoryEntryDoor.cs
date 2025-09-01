using Godot;

public partial class FactoryEntryDoor : Area3D, IInteractable
{
    [Export]
    public AudioStreamPlayer3D SfxLocked;

    public void Interact()
    {
        SfxLocked.Play();
        DialogueController.Instance.StartDialogue("##LOCKED##");
    }
}
