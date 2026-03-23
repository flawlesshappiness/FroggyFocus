using Godot;
using Godot.Collections;

public partial class HandInNpc : CharacterNpc
{
    [Export]
    public HandInInfo HandInInfo;

    [Export]
    public Array<string> RequestCompleteDialogueId;

    private HandInData HandInData => HandIn.GetOrCreateData(HandInInfo.Id);
    private int ClaimCount => HandInData.ClaimCount;

    private bool claimed_hand_in;

    public override void _Ready()
    {
        base._Ready();
        HandInController.Instance.OnHandInClaimed += HandInClaimed;
        HandInController.Instance.OnHandInClosed += HandInClosed;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        HandInController.Instance.OnHandInClaimed -= HandInClaimed;
        HandInController.Instance.OnHandInClosed -= HandInClosed;
    }

    public override void Interact()
    {
        base.Interact();

        claimed_hand_in = false;
        StartRequestCompleteDialogue();
    }

    protected override void DialogueEnded()
    {
        if (HasActiveDialogue)
        {
            var final_index = AllRequestsClaimed();
            var valid_hand_in = HandInInfo?.Requests?.Count > 0;
            if (!final_index && valid_hand_in)
            {
                HandInView.Instance.ShowPopup(HandInInfo.Id);
            }
            else
            {
                StopDialogueCamera();
            }
        }

        base.DialogueEnded();
    }

    private void HandInClaimed(string id)
    {
        if (id == HandInInfo.Id)
        {
            claimed_hand_in = true;
            StartRequestCompleteDialogue();
        }
    }

    private void HandInClosed(string id)
    {
        if (id == HandInInfo.Id)
        {
            StopDialogueCamera();
        }
    }

    private bool AllRequestsClaimed()
    {
        return ClaimCount >= HandInInfo.Requests?.Count;
    }

    private int GetDialogueNumber()
    {
        return Mathf.Clamp(ClaimCount, 0, RequestCompleteDialogueId.Count - 1);
    }

    private void StartRequestCompleteDialogue()
    {
        var dialogue_number = GetDialogueNumber();
        var dialogue_id = RequestCompleteDialogueId[dialogue_number];
        StartDialogue($"##{dialogue_id}##");
    }
}
