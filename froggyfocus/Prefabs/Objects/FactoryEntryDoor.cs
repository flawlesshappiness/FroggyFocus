using Godot;

public partial class FactoryEntryDoor : HouseDoor, IInteractable
{
    [Export]
    public bool StartLocked;

    [Export]
    public AudioStreamPlayer3D SfxLocked;

    public bool Locked { get; set; }

    public override void _Ready()
    {
        base._Ready();
        Locked = StartLocked;
    }

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
