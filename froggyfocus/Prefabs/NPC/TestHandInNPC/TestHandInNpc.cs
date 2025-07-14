using Godot;

public partial class TestHandInNpc : Area3D, IInteractable
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

    public void Interact()
    {
        if (HandIn.IsAvailable(HandInInfo.Id))
        {
            DialogueController.Instance.StartDialogue("##TEST_HAND_IN_001##");
        }
        else
        {
            DialogueController.Instance.StartDialogue("##TEST_HAND_IN_COMPLETED_001##");
        }
    }

    private void DialogueNodeEnded(string id)
    {
        if (id == "##TEST_HAND_IN_002##")
        {
            var data = HandIn.GetOrCreateData(HandInInfo.Id);
            HandInView.Instance.ShowPopup(data);
        }
    }

    private void HandInClaimed(string id)
    {
        if (id == HandInInfo.Id)
        {
            HandIn.ResetData(HandInInfo);
        }
    }
}
