using Godot;

public partial class FrogRaceNpc : CharacterNpc, IInteractable
{
    private const string DialogueIntro = "DEMO_RACE_FROG_INTRO";
    private const string DialogueRace = "DEMO_RACE_FROG_RACE";
    private const string DialogueWin = "DEMO_RACE_FROG_WIN";
    private const string DialogueLose = "DEMO_RACE_FROG_LOSE";

    [Export]
    public RaceTrack RaceTrack;

    [Export]
    public CuteFrogCharacter Character;

    private const string FlagIntro = "race_frog_intro";

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
        RaceGhost.SetupAppearance(Character);
    }

    public override void Interact()
    {
        base.Interact();

        if (GameFlags.IsFlag(FlagIntro, 0))
        {
            GameFlags.SetFlag(FlagIntro, 1);
            DialogueController.Instance.StartDialogue(DialogueIntro);
        }
        else
        {
            DialogueController.Instance.StartDialogue(DialogueRace);
        }
    }

    protected override void DialogueEnded(string id)
    {
        base.DialogueEnded(id);

        if (id == DialogueIntro || id == DialogueRace)
        {
            GameView.Instance.ShowPopup("##START_RACE##", "##YES##", "##NO##", () =>
            {
                StartRace();
            },
            () =>
            {

            });
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
