using FlawLizArt.Animation.StateMachine;
using Godot;
using System.Linq;

public partial class LeechNpc : Area3D, IInteractable
{
    [Export]
    public AnimationStateMachine Animation;

    [Export]
    public AudioStreamPlayer3D SfxSpeak;

    private int idx_dialogue;
    private bool active_dialogue;
    private readonly BoolParameter param_dialogue = new BoolParameter("dialogue", false);

    public override void _Ready()
    {
        base._Ready();
        DialogueController.Instance.OnNodeStarted += DialogueStarted;
        DialogueController.Instance.OnNodeEnded += DialogueEnded;
        DialogueController.Instance.OnDialogueEnded += DialogueEnded;

        InitializeAnimations();
    }

    private void InitializeAnimations()
    {
        var idle = Animation.CreateAnimation("Armature|idle", true);
        var look = Animation.CreateAnimation("Armature|dialogue", true);

        Animation.Connect(idle, look, param_dialogue.WhenTrue());
        Animation.Connect(look, idle, param_dialogue.WhenFalse());

        Animation.Start(idle.Node);
    }

    public void Interact()
    {
        var nodes = DialogueController.Instance.Collection.Nodes.Values.Where(x => x.id.Contains("LEECH_UPGRADE_START")).ToList();
        var node = nodes.GetClamped(idx_dialogue);
        idx_dialogue = (idx_dialogue + 1) % nodes.Count;
        StartDialogue(node.id);
    }

    private void StartDialogue(string id)
    {
        active_dialogue = true;
        param_dialogue.Set(true);
        DialogueController.Instance.StartDialogue(id);
    }

    private void DialogueStarted(string id)
    {
        if (active_dialogue)
        {
            SfxSpeak.Play();
        }
    }

    private void DialogueEnded(string id)
    {
        if (id.Contains("LEECH_UPGRADE_START"))
        {
            UpgradeView.Instance.Show();
        }
    }

    private void DialogueEnded()
    {
        param_dialogue.Set(false);
        active_dialogue = false;
    }
}
