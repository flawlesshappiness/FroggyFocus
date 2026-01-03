using Godot;

public partial class FrogPartnerNpc : CharacterNpc, IInteractable
{
    [Export]
    public HandInInfo HandInInfo;

    [Export]
    public FrogCharacter FrogCharacter;

    private HandInData HandInData => HandIn.GetOrCreateData(HandInInfo.Id);

    private readonly string DIALOGUE_ID = "PARTNER";

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

        if (HandInData.ClaimedCount > 0)
        {
            FrogCharacter.HatAttachments.SetAttachment(ItemType.Hat_BugOfLove, ItemType.Color_Default, ItemType.Color_Default);
        }
        else
        {
            FrogCharacter.HatAttachments.SetAttachment(ItemType.Hat_Bow, ItemType.Color_Default, ItemType.Color_Default);
        }
    }

    public override void Interact()
    {
        if (HandInData.ClaimedCount > 0)
        {
            StartDialogue($"##{DIALOGUE_ID}_REQUEST_COMPLETE_003##");
        }
        else if (MainQuestController.Instance.GetPartnerStep() == 0)
        {
            StartDialogue($"##{DIALOGUE_ID}_INTRO_001##");
        }
        else
        {
            StartDialogue($"##{DIALOGUE_ID}_REQUEST_001##");
        }
    }

    private void DialogueNodeEnded(string id)
    {
        if (id == $"##{DIALOGUE_ID}_INTRO_004##")
        {
            MainQuestController.Instance.AdvancePartnerQuest(1);
        }
        else if (id == $"##{DIALOGUE_ID}_REQUEST_002##")
        {
            HandInView.Instance.ShowPopup(HandInInfo.Id);
        }
        else if (id == $"##{DIALOGUE_ID}_REQUEST_COMPLETE_002##")
        {
            InitializeCharacter();
        }
    }

    private void HandInClaimed(string id)
    {
        if (id == HandInInfo.Id)
        {
            HandIn.ResetData(HandInInfo);
            Data.Game.Save();

            MainQuestController.Instance.AdvancePartnerQuest(5);
            StartDialogue($"##{DIALOGUE_ID}_REQUEST_COMPLETE_001##");
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
