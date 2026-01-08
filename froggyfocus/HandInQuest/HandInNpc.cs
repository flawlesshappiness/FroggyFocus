using Godot;
using Godot.Collections;

public partial class HandInNpc : CharacterNpc
{
    [Export]
    public HandInInfo HandInInfo;

    [Export]
    public string RequestDialogueId;

    [Export]
    public Array<string> RequestCompleteDialogueId;

    private bool claimed_hand_in;

    public override void _Ready()
    {
        base._Ready();
        HandIn.InitializeData(HandInInfo);
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
        if (HandIn.IsAvailable(HandInInfo.Id))
        {
            StartDialogue($"##{RequestDialogueId}##");
        }
        else
        {
            StartRequestCompleteDialogue();
        }
    }

    protected override void DialogueEnded()
    {
        if (HasActiveDialogue)
        {
            if (!claimed_hand_in)
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
            HandIn.ResetData(HandInInfo);
            Data.Game.Save();

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

    private int GetDialogueNumber()
    {
        var data = HandIn.GetOrCreateData(HandInInfo.Id);
        return Mathf.Clamp(data.ClaimedCount - 1, 0, RequestCompleteDialogueId.Count - 1);
    }

    private void StartRequestCompleteDialogue()
    {
        var dialogue_number = GetDialogueNumber();
        var dialogue_id = RequestCompleteDialogueId[dialogue_number];
        StartDialogue($"##{dialogue_id}##");
    }
}
