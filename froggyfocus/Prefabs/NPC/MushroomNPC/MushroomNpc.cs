using FlawLizArt.Animation.StateMachine;
using Godot;

public partial class MushroomNpc : Area3D, IInteractable
{
    [Export]
    public HandInInfo HandInInfo;

    [Export]
    public AnimationStateMachine Animation;

    private readonly BoolParameter param_dialogue = new BoolParameter("dialogue", false);

    public override void _Ready()
    {
        base._Ready();
        HandIn.InitializeData(HandInInfo);
        HandInController.Instance.OnHandInClaimed += HandInClaimed;

        DialogueController.Instance.OnNodeEnded += DialogueEnded;
        DialogueController.Instance.OnDialogueEnded += DialogueEnded;

        InitializeAnimations();
    }

    private void InitializeAnimations()
    {
        var idle = Animation.CreateAnimation("Armature|idle", true);
        var idle_dialogue = Animation.CreateAnimation("Armature|idle_dialogue", true);

        Animation.Connect(idle, idle_dialogue, param_dialogue.WhenTrue());
        Animation.Connect(idle_dialogue, idle, param_dialogue.WhenFalse());

        Animation.Start(idle.Node);
    }

    public void Interact()
    {
        if (HandIn.IsHandInAvailable(HandInInfo.Id))
        {
            DialogueController.Instance.StartDialogue("##MUSHROOM_SWAMP_REQUEST_001##");
        }
        else
        {
            DialogueController.Instance.StartDialogue("##MUSHROOM_SWAMP_REQUEST_IDLE_001##");
        }

        param_dialogue.Set(true);
    }

    private void HandInClaimed(string id)
    {
        if (id == HandInInfo.Id)
        {
            HandIn.ResetData(HandInInfo);
            DialogueController.Instance.StartDialogue("##MUSHROOM_SWAMP_REQUEST_COMPLETE_001##");
        }
    }

    private void DialogueEnded(string id)
    {
        if (id == "##MUSHROOM_SWAMP_REQUEST_002##")
        {
            var data = HandIn.GetOrCreateData(HandInInfo.Id);
            HandInView.Instance.ShowPopup(data);
        }
    }

    private void DialogueEnded()
    {
        param_dialogue.Set(false);
    }
}
