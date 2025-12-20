using Godot;

public partial class FrogManagerNpc : CharacterNpc, IInteractable
{
    [Export]
    public HandInInfo HandInInfo;

    [Export]
    public FrogCharacter FrogCharacter;

    private HandInData HandInData => HandIn.GetOrCreateData(HandInInfo.Id);

    private readonly string DIALOGUE_ID = "MANAGER";
    private string INTRO_ID => $"{DIALOGUE_ID}_INTRO";

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
        FrogCharacter.SetInSand();
        FrogCharacter.ClearAppearance();
        FrogCharacter.FaceAttachments.SetAttachment(ItemType.Face_Moustache, ItemType.Color_Default, ItemType.Color_Default);
    }

    public override void Interact()
    {
        if (HandInData.ClaimedCount > 0)
        {
            // TODO
            Debug.LogError("UNFINISHED");
        }
        else if (!GameFlags.HasFlag(INTRO_ID))
        {
            GameFlags.SetFlag(INTRO_ID, 1);
            StartDialogue($"##{DIALOGUE_ID}_INTRO_001##");
        }
        else
        {
            StartDialogue($"##{DIALOGUE_ID}_REQUEST_001##");
        }
    }

    private void DialogueNodeEnded(string id)
    {
        if (id == $"##{DIALOGUE_ID}_REQUEST_002##")
        {
            HandInView.Instance.ShowPopup(HandInInfo.Id);
        }
        else if (id == $"##{DIALOGUE_ID}_REQUEST_FAIL_002##")
        {
            StopDialogueCamera();
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
