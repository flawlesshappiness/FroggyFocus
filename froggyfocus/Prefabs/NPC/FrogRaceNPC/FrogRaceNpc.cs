using Godot;

public partial class FrogRaceNpc : CharacterNpc, IInteractable
{
    private const string DialogueIntro = "DEMO_RACE_FROG_INTRO";

    [Export]
    public RaceTrack RaceTrack;

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
}
