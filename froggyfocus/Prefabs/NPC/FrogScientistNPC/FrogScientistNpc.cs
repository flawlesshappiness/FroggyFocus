using Godot;

public partial class FrogScientistNpc : CharacterNpc, IInteractable
{
    [Export]
    public HandInInfo HandInInfo;

    [Export]
    public FrogCharacter FrogCharacter;

    private HandInData HandInData => HandIn.GetOrCreateData(HandInInfo.Id);

    private readonly string DIALOGUE_ID = "SCIENTIST";

    private bool show_unlock;

    public override void _Ready()
    {
        base._Ready();
        HandIn.InitializeData(HandInInfo);

        DialogueController.Instance.OnNodeEnded += DialogueNodeEnded;
        HandInController.Instance.OnHandInClaimed += HandInClaimed;
        HandInController.Instance.OnHandInClosed += HandInClosed;
    }

    public override void Interact()
    {
        if (HandInData.ClaimedCount > 0)
        {
            StartDialogue($"##{DIALOGUE_ID}_REQUEST_COMPLETE_003##");
        }
        else if (MainQuestController.Instance.GetScientistStep() == 0)
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
            MainQuestController.Instance.AdvanceScientistQuest(1);
        }
        else if (id == $"##{DIALOGUE_ID}_REQUEST_002##")
        {
            HandInView.Instance.ShowPopup(HandInInfo.Id);
        }
        else if (id == $"##{DIALOGUE_ID}_REQUEST_FAIL_002##")
        {
            StopDialogueCamera();
        }
        else if (id == $"##{DIALOGUE_ID}_REQUEST_COMPLETE_003##" && show_unlock)
        {
            Item.MakeOwned(ItemType.Particles_Hearts);
            Data.Game.Save();

            show_unlock = false;
            UnlockView.Instance.ShowItemUnlock(ItemType.Particles_Bubbles);
        }
    }

    private void HandInClaimed(string id)
    {
        if (id == HandInInfo.Id)
        {
            HandIn.ResetData(HandInInfo);
            Data.Game.ScientistQuestCompleted = true;
            Data.Game.Save();

            show_unlock = true;
            MainQuestController.Instance.AdvanceScientistQuest(4);
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
