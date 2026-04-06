using Godot;

public partial class FrogPartnerNpc : CharacterNpc, IInteractable
{
    [Export]
    public HandInInfo HandInInfo;

    [Export]
    public FrogCharacter FrogCharacter;

    private HandInData HandInData => HandIn.GetOrCreateData(HandInInfo.Id);

    private const string DialogueIntro = "PARTNER_INTRO";
    private const string DialogueRequest = "PARTNER_REQUEST";
    private const string DialogueRequestFail = "PARTNER_REQUEST_FAIL";
    private const string DialogueRequestComplete = "PARTNER_REQUEST_COMPLETE";
    private const string DialogueRequestCompleteRepeat = "PARTNER_REQUEST_COMPLETE_003";

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
        else if (MainQuestController.Instance.GetPartnerStep() == 0)
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
            MainQuestController.Instance.AdvancePartnerQuest(1);
        }
        else if (id == DialogueRequest)
        {
            HandInView.Instance.ShowPopup(HandInInfo.Id);
        }
        else if (id == DialogueRequestComplete || id == DialogueRequestCompleteRepeat)
        {
            if (show_unlock)
            {
                // TODO: Unlock something
                // Show unlock
                //InitializeCharacter();
                Data.Game.Save();
                show_unlock = false;
            }
        }
    }

    private void HandInClaimed(string id)
    {
        if (id == HandInInfo.Id)
        {
            show_unlock = true;
            Data.Game.PartnerQuestCompleted = true;
            MainQuestController.Instance.AdvancePartnerQuest(5);
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
