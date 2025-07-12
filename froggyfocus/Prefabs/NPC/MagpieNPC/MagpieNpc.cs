using FlawLizArt.Animation.StateMachine;
using Godot;

public partial class MagpieNpc : Area3D, IInteractable
{
    [Export]
    public AnimationStateMachine Animation;

    private bool active_dialogue;
    private BoolParameter param_dialogue = new BoolParameter("dialogue", false);

    public override void _Ready()
    {
        base._Ready();

        DialogueController.Instance.OnDialogueEnded += DialogueEnded;

        InitializeAnimations();
    }

    private void InitializeAnimations()
    {
        var idle = Animation.CreateAnimation("Armature|idle", true);
        var look = Animation.CreateAnimation("Armature|look", true);

        Animation.Connect(idle, look, param_dialogue.WhenTrue());
        Animation.Connect(look, idle, param_dialogue.WhenFalse());

        Animation.Start(idle.Node);
    }

    public void Interact()
    {
        StartDialogue("##TEST_001##");
    }

    private void StartDialogue(string id)
    {
        active_dialogue = true;
        param_dialogue.Set(true);
        DialogueController.Instance.StartDialogue(id);
    }

    private void DialogueEnded()
    {
        active_dialogue = false;
        param_dialogue.Set(false);
    }
}
