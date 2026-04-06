using Godot;

public partial class FrogDemoNpc : CharacterNpc, IInteractable
{
    [Export]
    public HandInInfo HandInInfo;

    private HandInData HandInData => HandIn.GetOrCreateData(HandInInfo.Id);

    private const string FlagIntro = "demo_frog_intro";
    private const string DialogueIntro = "DEMO_FROG_INTRO";
    private const string DialogueFlying = "DEMO_FROG_FLYING";
    private const string DialogueBlooming = "DEMO_FROG_BLOOMING";
    private const string DialogueFinished = "DEMO_FROG_FINISHED";
    private const string DialogueFinishedRepeat = "DEMO_FROG_FINISHED_004";
    private const string DialogueTask = "DEMO_FROG_TASK_001";
    private const string DialogueCatch = "DEMO_FROG_CATCH_001";

    public override void _Ready()
    {
        base._Ready();
        HandInController.Instance.OnHandInClaimed += HandInClaimed;
        HandInController.Instance.OnHandInClosed += HandInClosed;
    }

    public override void Interact()
    {
        if (GameFlags.IsFlag(FlagIntro, 0))
        {
            StartDialogue(DialogueIntro);
        }
        else if (HandInData.ClaimCount >= 3)
        {
            StartDialogue(DialogueFinishedRepeat);
        }
        else
        {
            StartDialogue(DialogueTask);
        }
    }

    protected override void DialogueEnded(string id)
    {
        base.DialogueEnded(id);

        if (id == DialogueIntro)
        {
            GameFlags.SetFlag(FlagIntro, 1);
            HandInView.Instance.ShowPopup(HandInInfo.Id);
        }
        else if (id == DialogueTask)
        {
            HandInView.Instance.ShowPopup(HandInInfo.Id);
        }
        else if (id == DialogueFlying)
        {
            HandInView.Instance.ShowPopup(HandInInfo.Id);
        }
        else if (id == DialogueBlooming)
        {
            HandInView.Instance.ShowPopup(HandInInfo.Id);
        }
        else if (id == DialogueFinished || id == DialogueFinishedRepeat)
        {
            StopDialogueCamera();
            DemoView.Instance.AnimateShow();
        }
    }

    private void HandInClaimed(string id)
    {
        if (id != HandInInfo.Id) return;

        if (HandInData.ClaimCount == 1)
        {
            StartDialogue(DialogueFlying);
        }
        else if (HandInData.ClaimCount == 2)
        {
            StartDialogue(DialogueBlooming);
        }
        else if (HandInData.ClaimCount == 3)
        {
            StartDialogue(DialogueFinished);
        }
    }

    private void HandInClosed(string id)
    {
        if (id != HandInInfo.Id) return;

        if (HandInData.ClaimCount == 0)
        {
            StartDialogue(DialogueCatch);
        }

        StopDialogueCamera();
    }
}
