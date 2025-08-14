using System.Linq;

public partial class LeechNpc : CharacterNpc
{
    private int idx_dialogue;
    private bool active_dialogue;

    public override void _Ready()
    {
        base._Ready();
        DialogueController.Instance.OnNodeEnded += DialogueNodeEnded;
    }

    public override void Interact()
    {
        var nodes = DialogueController.Instance.Collection.Nodes.Values.Where(x => x.id.Contains("LEECH_UPGRADE_START")).ToList();
        var node = nodes.GetClamped(idx_dialogue);
        idx_dialogue = (idx_dialogue + 1) % nodes.Count;
        StartDialogue(node.id);
    }

    private void DialogueNodeEnded(string id)
    {
        if (id.Contains("LEECH_UPGRADE_START"))
        {
            UpgradeView.Instance.Show();
        }
    }
}
