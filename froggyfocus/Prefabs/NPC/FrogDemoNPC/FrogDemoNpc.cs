using Godot;

public partial class FrogDemoNpc : CharacterNpc, IInteractable
{
    [Export]
    public HandInInfo HandInInfo;

    private HandInData HandInData => HandIn.GetOrCreateData(HandInInfo.Id);
    private string IntroDialogueId => "demo_frog_intro";

    public override void _Ready()
    {
        base._Ready();
        DialogueController.Instance.OnNodeEnded += DialogueNodeEnded;
        HandInController.Instance.OnHandInClaimed += HandInClaimed;
        HandInController.Instance.OnHandInClosed += HandInClosed;
    }

    public override void Interact()
    {
        if (GameFlags.IsFlag(IntroDialogueId, 0))
        {
            StartDialogue("##DEMO_FROG_INTRO_001##");
        }
        else if (HandInData.ClaimCount >= 3)
        {
            StartDialogue("##DEMO_FROG_FINISHED_004##");
        }
        else
        {
            StartDialogue("##DEMO_FROG_TASK_001##");
        }
    }

    private void DialogueNodeEnded(string id)
    {
        if (id == "##DEMO_FROG_INTRO_004##")
        {
            GameFlags.SetFlag(IntroDialogueId, 1);
            HandInView.Instance.ShowPopup(HandInInfo.Id);
        }
        else if (id == "##DEMO_FROG_TASK_001##")
        {
            HandInView.Instance.ShowPopup(HandInInfo.Id);
        }
        else if (id == "##DEMO_FROG_FLYING_003##")
        {
            HandInView.Instance.ShowPopup(HandInInfo.Id);
        }
        else if (id == "##DEMO_FROG_BLOOMING_004##")
        {
            HandInView.Instance.ShowPopup(HandInInfo.Id);
        }
        else if (id == "##DEMO_FROG_FINISHED_004##")
        {
            StopDialogueCamera();
            DemoView.Instance.AnimateShow();
        }
    }

    private void HandInClaimed(string id)
    {
        if (HandInData.ClaimCount == 1)
        {
            StartDialogue("##DEMO_FROG_FLYING_001##");
        }
        else if (HandInData.ClaimCount == 2)
        {
            StartDialogue("##DEMO_FROG_BLOOMING_001##");
        }
        else if (HandInData.ClaimCount == 3)
        {
            StartDialogue("##DEMO_FROG_FINISHED_001##");
        }
    }

    private void HandInClosed(string id)
    {
        if (HandInData.ClaimCount == 0)
        {
            StartDialogue("##DEMO_FROG_CATCH_001##");
        }

        StopDialogueCamera();
    }
}
