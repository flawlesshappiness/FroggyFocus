using FlawLizArt.Animation.StateMachine;
using Godot;

public partial class OtterNpc : Area3D, IInteractable
{
    [Export]
    public HandInInfo HandInInfo;

    [Export]
    public AnimationStateMachine Animation;

    [Export]
    public AudioStreamPlayer3D SfxSpeak;

    private bool active_dialogue;
    private readonly BoolParameter param_dialogue = new BoolParameter("dialogue", false);

    public override void _Ready()
    {
        base._Ready();
        HandIn.InitializeData(HandInInfo);
        HandInController.Instance.OnHandInClaimed += HandInClaimed;

        DialogueController.Instance.OnNodeStarted += DialogueStarted;
        DialogueController.Instance.OnNodeEnded += DialogueEnded;
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
        if (HandIn.IsAvailable(HandInInfo.Id))
        {
            StartDialogue("##OTTER_SWAMP_REQUEST_001##");
        }
        else
        {
            StartDialogue("##OTTER_SWAMP_REQUEST_IDLE_001##");
        }
    }

    private void HandInClaimed(string id)
    {
        if (id == HandInInfo.Id)
        {
            HandIn.ResetData(HandInInfo);
            Data.Game.Save();

            StartDialogue("##OTTER_SWAMP_REQUEST_COMPLETE_001##");
        }
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
        if (id == "##OTTER_SWAMP_REQUEST_002##")
        {
            var data = HandIn.GetOrCreateData(HandInInfo.Id);
            HandInView.Instance.ShowPopup(data);
        }
    }

    private void DialogueEnded()
    {
        param_dialogue.Set(false);
        active_dialogue = false;
    }
}
