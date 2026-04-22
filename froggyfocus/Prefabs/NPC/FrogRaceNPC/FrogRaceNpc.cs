using Godot;

public partial class FrogRaceNpc : CharacterNpc, IInteractable
{
    [Export]
    public RaceInfo Info;

    [Export]
    public RaceTrack RaceTrack;

    [Export]
    public CuteFrogCharacter Character;

    private bool IsEasy => GameFlags.IsFlag(Info.Id, 1);

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

        if (GameFlags.IsFlag(Info.Id, 0))
        {
            GameFlags.SetFlag(Info.Id, 1);
            Data.Game.Save();
            StartDialogue(Info.DialogueEasy);
        }
        else if (GameFlags.IsFlag(Info.Id, 2))
        {
            GameFlags.SetFlag(Info.Id, 3);
            Data.Game.Save();
            StartDialogue(Info.DialogueHard);
        }
        else
        {
            StartDialogue(Info.DialogueRace);
        }
    }

    protected override void DialogueEnded(string id)
    {
        base.DialogueEnded(id);

        if (id == Info.DialogueEasy || id == Info.DialogueHard || id == Info.DialogueRace)
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
        var ghost = IsEasy ? Info.GhostEasy : Info.GhostHard;

        var settings = new RaceSettings
        {
            GhostInfo = ghost,
            Track = RaceTrack
        };

        RaceController.Instance.StartRace(settings);
    }

    private void RaceEnd(RaceResult result)
    {
        if (result.IsWin)
        {
            var dialogue = IsEasy ? Info.DialogueWinEasy : Info.DialogueWinHard;
            StartDialogue(dialogue);

            if (GameFlags.IsFlag(Info.Id, 1)) GameFlags.SetFlag(Info.Id, 2);
            else if (GameFlags.IsFlag(Info.Id, 3)) GameFlags.SetFlag(Info.Id, 4);
            Data.Game.Save();
        }
        else
        {
            StartDialogue(Info.DialogueLose);
        }
    }
}
