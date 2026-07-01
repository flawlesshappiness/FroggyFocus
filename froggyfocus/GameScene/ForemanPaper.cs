using Godot;

public partial class ForemanPaper : Area3D, IInteractable
{
    [Export]
    public string DialogueNode;

    [Export]
    public bool IsForemanCode;

    public void Interact()
    {
        if (IsForemanCode)
        {
            GameFlags.SetFlag(CrystalDoorway.HasCodeFlag, 1);
            Data.Game.Save();
        }

        DialogueController.Instance.StartDialogue(DialogueNode);
    }
}
