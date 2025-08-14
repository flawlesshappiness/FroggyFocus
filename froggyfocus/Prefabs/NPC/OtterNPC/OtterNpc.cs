using Godot;

public partial class OtterNpc : CharacterNpc
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

    public override void Interact()
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

    private void DialogueNodeEnded(string id)
    {
        if (id == "##OTTER_SWAMP_REQUEST_002##")
        {
            var data = HandIn.GetOrCreateData(HandInInfo.Id);
            HandInView.Instance.ShowPopup(data);
        }
    }
}
