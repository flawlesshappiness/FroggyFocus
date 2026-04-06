using Godot;

public partial class FrogScientistNpc : CharacterNpc, IInteractable
{
    [Export]
    public HandInInfo HandInInfo;

    private HandInData HandInData => HandIn.GetOrCreateData(HandInInfo.Id);

    private const string DialogueIntro = "SCIENTIST_INTRO";
    private const string DialogueRequest = "SCIENTIST_REQUEST";
    private const string DialogueRequestFail = "SCIENTIST_REQUEST_FAIL";
    private const string DialogueRequestComplete = "SCIENTIST_REQUEST_COMPLETE";
    private const string DialogueRequestCompleteRepeat = "SCIENTIST_REQUEST_COMPLETE_003";

    private bool show_unlock;

    public override void _Ready()
    {
        base._Ready();
        HandInController.Instance.OnHandInClaimed += HandInClaimed;
        HandInController.Instance.OnHandInClosed += HandInClosed;
    }

    public override void Interact()
    {
        if (HandInData.ClaimCount > 0)
        {
            StartDialogue(DialogueRequestCompleteRepeat);
        }
        else if (MainQuestController.Instance.GetScientistStep() == 0)
        {
            StartDialogue(DialogueIntro);
        }
        else
        {
            StartDialogue(DialogueRequest);
        }
    }

    protected override void DialogueEnded(string id)
    {
        base.DialogueEnded(id);

        if (id == DialogueIntro)
        {
            MainQuestController.Instance.AdvanceScientistQuest(1);
        }
        else if (id == DialogueRequest)
        {
            HandInView.Instance.ShowPopup(HandInInfo.Id);
        }
        else if (id == DialogueRequestFail)
        {
            StopDialogueCamera();
        }
        else if (id == DialogueRequestComplete)
        {
            if (show_unlock)
            {
                // TODO: Unlock something
                Data.Game.Save();

                show_unlock = false;
                //UnlockView.Instance.ShowItemUnlock(ItemType.Particles_Bubbles);
            }
        }
    }

    private void HandInClaimed(string id)
    {
        if (id == HandInInfo.Id)
        {
            show_unlock = true;
            Data.Game.ScientistQuestCompleted = true;
            MainQuestController.Instance.AdvanceScientistQuest(4);
            StartDialogue(DialogueRequestComplete);
        }
    }

    private void HandInClosed(string id)
    {
        if (id == HandInInfo.Id)
        {
            StartDialogue(DialogueRequestFail);
        }
    }
}
