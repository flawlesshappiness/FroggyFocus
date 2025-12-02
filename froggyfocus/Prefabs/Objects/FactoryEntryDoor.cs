using Godot;

public partial class FactoryEntryDoor : HouseDoor, IInteractable
{
    [Export]
    public AudioStreamPlayer3D SfxLocked;

    public bool Locked { get; set; }

    public override void Interact()
    {
        if (Locked)
        {
            SfxLocked.Play();
            DialogueController.Instance.StartDialogue("##LOCKED##");
        }
        else
        {
            base.Interact();
        }
    }
}
