using Godot;

public partial class FrogPartnerNpc : CharacterNpc, IInteractable
{
    [Export]
    public HandInInfo HandInInfo;

    [Export]
    public CuteFrogCharacter FrogCharacter;

    [Export]
    public Color BodyColor;

    [Export]
    public Color TopColor;

    [Export]
    public Marker3D DefaultFacingMarker;

    private HandInData HandInData => HandIn.GetOrCreateData(HandInInfo.Id);

    private const string DialogueIntro = "PARTNER_INTRO";
    private const string DialogueRequest = "PARTNER_REQUEST";
    private const string DialogueRequestFail = "PARTNER_REQUEST_FAIL";
    private const string DialogueRequestComplete = "PARTNER_REQUEST_COMPLETE";
    private const string DialogueRequestCompleteRepeat = "PARTNER_REQUEST_COMPLETE_003";

    public override void _Ready()
    {
        base._Ready();
        HandInController.Instance.OnHandInClaimed += HandInClaimed;
        HandInController.Instance.OnHandInClosed += HandInClosed;

        ClearFacingDirection();
        UpdateAppearance();
    }

    private void UpdateAppearance()
    {
        FrogCharacter.ClearAppearance();
        FrogCharacter.SetBodyBase(BodyColor);
        FrogCharacter.SetBodyEye(ItemType.BodyEye_Cute, Colors.White);
        FrogCharacter.SetBodyTop(ItemType.BodyTop_Gradient, TopColor);
        FrogCharacter.SetBodyPattern(ItemType.BodyPattern_None, Colors.Transparent);

        if (HandInData.ClaimCount == 0)
        {
            FrogCharacter.SetAppearanceAttachment(ItemCategory.Hat, ItemType.Hat_Bow, Colors.Red, Colors.DarkRed);
        }
        else
        {
            FrogCharacter.SetAppearanceAttachment(ItemCategory.Hat, ItemType.Hat_BugOfLove, new Color(0f, 0.5f, 1f), Colors.Red);
        }
    }

    private void ClearFacingDirection()
    {
        FrogCharacter.StartFacingPosition(DefaultFacingMarker.GlobalPosition);
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

        FrogCharacter.StartFacingPosition(Player.Instance.GlobalPosition);
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
            UpdateAppearance();
        }

        ClearFacingDirection();
    }

    private void HandInClaimed(string id)
    {
        if (id == HandInInfo.Id)
        {
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

    public void StartToolboxDialogue()
    {
        StartDialogue("PARTNER_TOOLBOX");
        FrogCharacter.StartFacingPosition(Player.Instance.GlobalPosition);
    }
}
