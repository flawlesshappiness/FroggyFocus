using Godot;
using Godot.Collections;
using System.Linq;

public partial class LeechNpc : CharacterNpc
{
    [Export]
    public Array<string> Dialogues;

    private int idx_dialogue;
    private bool active_dialogue;

    public override void _Ready()
    {
        base._Ready();
        DialogueController.Instance.OnEntryEnded += DialogueNodeEnded;
    }

    public override void Interact()
    {
        var dialogue = Dialogues.PickRandom();
        StartDialogue(dialogue);
    }

    private void DialogueNodeEnded(string id)
    {
        if (Dialogues.Any(x => x == id))
        {
            UpgradeView.Instance.Show();
        }
    }
}
