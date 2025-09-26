using Godot;

public partial class FrogManagerNpc : Area3D, IInteractable
{
    [Export]
    public HandInInfo HandInInfo;

    [Export]
    public FrogCharacter FrogCharacter;

    private HandInData HandInData => HandIn.GetOrCreateData(HandInInfo.Id);

    private bool active_dialogue;

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
        FrogCharacter.FaceAttachments.SetAttachment(ItemType.Face_Moustache);
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
            StartDialogue("##MANAGER_REQUEST_001##");
        }
    }

    private void StartDialogue(string id)
    {
        active_dialogue = true;
        DialogueController.Instance.StartDialogue(id);
    }

    private void DialogueNodeEnded(string id)
    {
        if (id == "##MANAGER_REQUEST_002##")
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
            StartDialogue("##MANAGER_REQUEST_FAIL_001##");
        }
    }
}
