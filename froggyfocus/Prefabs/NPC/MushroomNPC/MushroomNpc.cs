using Godot;

public partial class MushroomNpc : CharacterNpc
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
            StartDialogue("##MUSHROOM_SWAMP_REQUEST_001##");
        }
        else
        {
            StartDialogue("##MUSHROOM_SWAMP_REQUEST_IDLE_001##");
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
        if (id == "##MUSHROOM_SWAMP_REQUEST_002##")
        {
            var data = HandIn.GetOrCreateData(HandInInfo.Id);
            HandInView.Instance.ShowPopup(data);
        }
    }
}
