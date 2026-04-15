using Godot;

public partial class FrogRaceNpc : CharacterNpc, IInteractable
{
    private const string DialogueIntro = "DEMO_RACE_FROG_INTRO";
    private const string DialogueWin = "DEMO_RACE_FROG_WIN";
    private const string DialogueLose = "DEMO_RACE_FROG_LOSE";

    [Export]
    public RaceTrack RaceTrack;

    [Export]
    public FrogCharacter Character;

    public override void _Ready()
    {
        base._Ready();
        RaceController.Instance.OnRaceEnd += RaceEnd;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        RaceController.Instance.OnRaceEnd -= RaceEnd;
    }

    protected override void Initialize()
    {
        base.Initialize();
        Character.SetAppearanceAttachment(ItemCategory.Hat, ItemType.Hat_Crown, Colors.Red, Colors.Yellow);
    }

    public override void Interact()
    {
        base.Interact();
        DialogueController.Instance.StartDialogue(DialogueIntro);
    }

    protected override void DialogueEnded(string id)
    {
        base.DialogueEnded(id);

        if (id == DialogueIntro)
        {
            StartRace();
        }
    }

    private void StartRace()
    {
        var settings = new RaceSettings
        {
            Track = RaceTrack
        };

        RaceController.Instance.StartRace(settings);
    }

    private void RaceEnd(RaceResult result)
    {
        if (result.IsWin)
        {
            StartDialogue(DialogueWin);
        }
        else
        {
            StartDialogue(DialogueLose);
        }
    }
}
