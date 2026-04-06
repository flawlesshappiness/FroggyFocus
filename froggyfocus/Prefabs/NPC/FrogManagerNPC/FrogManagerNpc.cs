using Godot;

public partial class FrogManagerNpc : CharacterNpc, IInteractable
{
    [Export]
    public HandInInfo HandInInfo;

    [Export]
    public FrogCharacter FrogCharacter;

    private HandInData HandInData => HandIn.GetOrCreateData(HandInInfo.Id);

    private const string DialogueIntro = "MANAGER_INTRO";
    private const string DialogueRequest = "MANAGER_REQUEST";
    private const string DialogueRequestFail = "MANAGER_REQUEST_FAIL";
    private const string DialogueRequestComplete = "MANAGER_REQUEST_COMPLETE";
    private const string DialogueRequestCompleteRepeat = "MANAGER_REQUEST_COMPLETE_003";

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
        else if (MainQuestController.Instance.GetManagerStep() == 0)
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

        if (id == DialogueRequest)
        {
            HandInView.Instance.ShowPopup(HandInInfo.Id);
        }
        else if (id == DialogueRequestFail)
        {
            StopDialogueCamera();
        }
        else if (id == DialogueIntro)
        {
            StopDialogueCamera();
            MainQuestController.Instance.AdvanceManagerQuest(1);
        }
        else if (id == DialogueRequestComplete || id == DialogueRequestCompleteRepeat)
        {
            StopDialogueCamera();

            if (show_unlock)
            {
                // TODO: Unlock something
                // Show unlock
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
            Data.Game.ManagerQuestCompleted = true;
            MainQuestController.Instance.AdvanceManagerQuest(5);
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
