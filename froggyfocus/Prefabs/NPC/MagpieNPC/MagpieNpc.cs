using FlawLizArt.Animation.StateMachine;
using Godot;

public partial class MagpieNpc : Area3D, IInteractable
{
    [Export]
    public AnimationStateMachine Animation;

    [Export]
    public AudioStreamPlayer3D SfxSpeak;

    private bool active_dialogue;
    private BoolParameter param_dialogue = new BoolParameter("dialogue", false);

    public override void _Ready()
    {
        base._Ready();

        DialogueController.Instance.OnDialogueEnded += DialogueEnded;
        DialogueController.Instance.OnNodeStarted += DialogueNodeStarted;

        InitializeAnimations();
    }

    private void InitializeAnimations()
    {
        var idle = Animation.CreateAnimation("Armature|idle", true);
        var idle_look = Animation.CreateAnimation("Armature|idle_look", false);
        var look = Animation.CreateAnimation("Armature|look", true);
        var look_tilt = Animation.CreateAnimation("Armature|look_tilt", false);
        var nod = Animation.CreateAnimation("Armature|nod", false);

        idle.AddVariation(idle_look, 0.25f);
        look.AddVariation(look_tilt, 0.25f);

        Animation.Connect(idle, look, param_dialogue.WhenTrue());
        Animation.Connect(look, idle, param_dialogue.WhenFalse());

        Animation.Connect(idle_look, idle);
        Animation.Connect(look_tilt, look);

        Animation.Connect(nod, look, param_dialogue.WhenTrue());
        Animation.Connect(nod, idle, param_dialogue.WhenFalse());

        Animation.Start(idle.Node);
    }

    public void Interact()
    {
        StartDialogue("##MAGPIE_SWAMP_FETCH_001##");
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

    private void DialogueNodeStarted(string id)
    {
        if (!active_dialogue) return;

        SfxSpeak.Play();
    }
}
