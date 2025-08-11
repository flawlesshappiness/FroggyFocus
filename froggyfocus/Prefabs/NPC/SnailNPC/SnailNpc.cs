using Godot;

public partial class SnailNpc : CharacterNpc, IInteractable
{
    [Export]
    public HandInInfo HandInInfo;

    public override void _Ready()
    {
        base._Ready();
        HandIn.InitializeData(HandInInfo);
        HandInController.Instance.OnHandInClaimed += HandInClaimed;
        DialogueController.Instance.OnNodeEnded += DialogueNodeEnded;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        HandInController.Instance.OnHandInClaimed -= HandInClaimed;
        DialogueController.Instance.OnNodeEnded -= DialogueNodeEnded;
    }

    public override void Interact()
    {
        base.Interact();
        if (HandIn.IsAvailable(HandInInfo.Id))
        {
            StartDialogue("##SNAIL_REQUEST_001##");
        }
        else
        {
            StartDialogue("##SNAIL_IDLE_001##");
        }
    }

    private void HandInClaimed(string id)
    {
        if (id == HandInInfo.Id)
        {
            HandIn.ResetData(HandInInfo);
            Data.Game.Save();

            StartDialogue("##MUSHROOM_SWAMP_REQUEST_COMPLETE_001##");
        }
    }

    private void DialogueNodeEnded(string id)
    {
        if (id == "##SNAIL_REQUEST_001##")
        {
            HandInView.Instance.ShowPopup(HandInInfo.Id);
        }
    }
}
