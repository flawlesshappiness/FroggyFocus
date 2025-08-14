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
    protected AnimationState IdleState { get; set; }
    protected AnimationState DialogueState { get; set; }

    protected BoolParameter param_dialogue = new BoolParameter("dialogue", false);

    public override void _Ready()
    {
        base._Ready();

        DialogueController.Instance.OnNodeStarted += DialogueStarted;
        DialogueController.Instance.OnDialogueEnded += DialogueEnded;

        InitializeAnimations();
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        DialogueController.Instance.OnNodeStarted -= DialogueStarted;
        DialogueController.Instance.OnDialogueEnded -= DialogueEnded;
    }

    protected virtual void InitializeAnimations()
    {
        IdleState = Animation.CreateAnimation(IdleAnimation, true);
        DialogueState = Animation.CreateAnimation(DialogueAnimation, true);

        Animation.Connect(IdleState, DialogueState, param_dialogue.WhenTrue());
        Animation.Connect(DialogueState, IdleState, param_dialogue.WhenFalse());

        Animation.Start(IdleState.Node);
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
