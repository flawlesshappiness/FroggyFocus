using Godot;

public partial class RaccoonNpc : CharacterNpc
{
    [Export]
    public HandInInfo HandInInfo;

    public override void _Ready()
    {
        base._Ready();
        HandIn.InitializeData(HandInInfo);
        DialogueController.Instance.OnNodeEnded += DialogueNodeEnded;
        HandInController.Instance.OnHandInClaimed += HandInClaimed;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        DialogueController.Instance.OnNodeEnded -= DialogueNodeEnded;
        HandInController.Instance.OnHandInClaimed -= HandInClaimed;
    }

    public override void Interact()
    {
        base.Interact();
        if (HandIn.IsAvailable(HandInInfo.Id))
        {
            StartDialogue("##RACCOON_REQUEST_001##");
        }
        else
        {
            StartDialogue("##RACCOON_IDLE_001##");
        }
    }

    private void DialogueNodeEnded(string id)
    {
        if (id == "##RACCOON_REQUEST_002##")
        {
            HandInView.Instance.ShowPopup(HandInInfo.Id);
        }
    }

    private void HandInClaimed(string id)
    {
        if (id == HandInInfo.Id)
        {
            HandIn.ResetData(HandInInfo);
            Data.Game.Save();

            StartDialogue("##RACCOON_REQUEST_COMPLETE_001##");
        }
    }
}
