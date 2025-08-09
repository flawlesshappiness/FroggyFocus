using FlawLizArt.Animation.StateMachine;
using Godot;

public partial class CharacterNpc : Area3D, IInteractable
{
    [Export]
    public string IdleAnimation = "Armature|idle";

    [Export]
    public string DialogueAnimation = "Armature|dialogue";

    [Export]
    public AnimationStateMachine Animation;

    [Export]
    public AudioStreamPlayer3D SfxSpeak;

    protected bool HasActiveDialogue { get; set; }

    protected BoolParameter param_dialogue = new BoolParameter("dialogue", false);

    public override void _Ready()
    {
        base._Ready();

        DialogueController.Instance.OnNodeStarted += DialogueStarted;
        DialogueController.Instance.OnDialogueEnded += DialogueEnded;

        InitializeAnimations();
    }

    protected virtual void InitializeAnimations()
    {
        var idle = Animation.CreateAnimation(IdleAnimation, true);
        var look = Animation.CreateAnimation(DialogueAnimation, true);

        Animation.Connect(idle, look, param_dialogue.WhenTrue());
        Animation.Connect(look, idle, param_dialogue.WhenFalse());

        Animation.Start(idle.Node);
    }

    public virtual void Interact()
    {

    }

    protected void StartDialogue(string id)
    {
        HasActiveDialogue = true;
        param_dialogue.Set(true);
        DialogueController.Instance.StartDialogue(id);
    }

    private void DialogueStarted(string id)
    {
        if (HasActiveDialogue)
        {
            SfxSpeak.Play();
        }
    }

    private void DialogueEnded()
    {
        param_dialogue.Set(false);
        HasActiveDialogue = false;
    }
}
