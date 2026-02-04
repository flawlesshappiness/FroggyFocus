using Godot;
using Godot.Collections;
using System.Linq;

public partial class FrogWorkerNpc : CharacterNpc, IInteractable
{
    [Export]
    public string DialogueFlag;

    [Export]
    public FrogCharacter FrogCharacter;

    [Export]
    public Array<string> DialogueNodes;

    private bool _initialized;

    public override void Interact()
    {
        base.Interact();
        var id_dialogue = DialogueNodes.ToList().GetClamped(GameFlags.GetFlag(DialogueFlag));
        DialogueController.Instance.StartDialogue(id_dialogue);

        GameFlags.IncrementFlag(DialogueFlag);
        Data.Game.Save();
    }
}