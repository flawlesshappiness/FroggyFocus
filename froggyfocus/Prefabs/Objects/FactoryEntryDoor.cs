using Godot;
using System;

public partial class FactoryEntryDoor : HouseDoor, IInteractable
{
    [Export]
    public bool StartLocked;

    [Export]
    public AudioStreamPlayer3D SfxLocked;

    public bool Locked { get; set; }

    public event Action OnInteractLocked;

    private bool active_dialogue;
    private const string DialogueLocked = "LOCKED";

    public override void _Ready()
    {
        base._Ready();
        Locked = StartLocked;
        DialogueController.Instance.OnDialogueEnded += Dialogue_Ended;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        DialogueController.Instance.OnDialogueEnded -= Dialogue_Ended;
    }

    public override void Interact()
    {
        if (Locked)
        {
            SfxLocked.Play();
            active_dialogue = true;
            DialogueController.Instance.StartDialogue(DialogueLocked);
        }
        else
        {
            base.Interact();
        }
    }

    private void Dialogue_Ended(string id)
    {
        if (!active_dialogue) return;

        if (Locked)
        {
            OnInteractLocked?.Invoke();
        }

        active_dialogue = false;
    }
}
