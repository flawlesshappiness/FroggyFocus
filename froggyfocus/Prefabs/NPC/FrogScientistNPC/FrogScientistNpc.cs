using Godot;

public partial class FrogScientistNpc : Area3D, IInteractable
{
    [Export]
    public HandInInfo HandInInfo;

    [Export]
    public FrogCharacter FrogCharacter;

    private HandInData HandInData => HandIn.GetOrCreateData(HandInInfo.Id);

    private bool active_dialogue;

    private readonly string DIALOGUE_ID = "SCIENTIST";

    public override void _Ready()
    {
        base._Ready();
        HandIn.InitializeData(HandInInfo);
        InitializeCharacter();

        DialogueController.Instance.OnNodeEnded += DialogueNodeEnded;
        HandInController.Instance.OnHandInClaimed += HandInClaimed;
        HandInController.Instance.OnHandInClosed += HandInClosed;
    }

    private void InitializeCharacter()
    {
        FrogCharacter.ClearAppearance();
        FrogCharacter.FaceAttachments.SetAttachment(ItemType.Face_Glasses_Scientist, ItemType.Color_Default, ItemType.Color_Default);
    }

    public void Interact()
    {
        if (HandInData.ClaimedCount > 0)
        {
            // TODO
            Debug.LogError("UNFINISHED");
        }
        else
        {
            StartDialogue($"##{DIALOGUE_ID}_REQUEST_001##");
        }
    }

    private void StartDialogue(string id)
    {
        active_dialogue = true;
        DialogueController.Instance.StartDialogue(id);
    }

    private void DialogueNodeEnded(string id)
    {
        if (id == $"##{DIALOGUE_ID}_REQUEST_002##")
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

            // TODO
            Debug.LogError("UNFINISHED");
            //StartDialogue("##CRAB_REQUEST_COMPLETE_001##");
        }
    }

    private void HandInClosed(string id)
    {
        if (id == HandInInfo.Id)
        {
            StartDialogue($"##{DIALOGUE_ID}_REQUEST_FAIL_001##");
        }
    }
}
