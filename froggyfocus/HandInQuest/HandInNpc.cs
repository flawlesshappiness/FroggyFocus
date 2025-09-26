using Godot;

public partial class HandInNpc : CharacterNpc
{
    [Export]
    public HandInInfo HandInInfo;

    [Export]
    public string DialogueKey;

    private bool claimed_hand_in;

    public override void _Ready()
    {
        base._Ready();
        HandIn.InitializeData(HandInInfo);
        HandInController.Instance.OnHandInClaimed += HandInClaimed;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        HandInController.Instance.OnHandInClaimed -= HandInClaimed;
    }

    public override void Interact()
    {
        base.Interact();

        claimed_hand_in = false;
        if (HandIn.IsAvailable(HandInInfo.Id))
        {
            StartDialogue($"##{DialogueKey}_REQUEST##");
        }
        else
        {
            StartRequestCompleteDialogue();
        }
    }

    protected override void DialogueEnded()
    {
        if (HasActiveDialogue && !claimed_hand_in)
        {
            HandInView.Instance.ShowPopup(HandInInfo.Id);
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

    private string GetDialogueNumber()
    {
        var data = HandIn.GetOrCreateData(HandInInfo.Id);
        var max = Mathf.Max(HandInInfo.ClaimCountToUnlock, 1);
        return (1 + data.ClaimedCount % max).ToString("000");
    }

    private void StartRequestCompleteDialogue()
    {
        var dialogue_number = GetDialogueNumber();
        StartDialogue($"##{DialogueKey}_COMPLETE_{dialogue_number}##");
    }
}
