using Godot;
using System.Collections.Generic;

public partial class ReceptionVent : Node3D
{
    [Export]
    public VentDoor Vent;

    [Export]
    public Node3D VentCoverLocked;

    [Export]
    public Node3D VentCoverUnlocked;

    [Export]
    public AudioStreamPlayer SfxUnlock;

    public const string FlagUnlocked = "vent_reception_unlocked";
    public const string FlagHasScrewdriver = "vent_screwdriver";
    private readonly string DialogueLocked = "VENT_LOCKED";

    private bool IsUnlocked => GameFlags.IsFlag(FlagUnlocked, 1);
    private bool HasScrewdriver => GameFlags.IsFlag(FlagHasScrewdriver, 1);

    public override void _Ready()
    {
        base._Ready();
        UpdateLocked();
        Vent.OnInteractLocked += Vent_InteractLocked;
        DialogueController.Instance.OnDialogueEnded += Dialogue_Ended;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        DialogueController.Instance.OnDialogueEnded -= Dialogue_Ended;
    }

    private void Dialogue_Ended(string id)
    {
        if (id == DialogueLocked)
        {
            ShowScrewOptions();
        }
    }

    private void Vent_InteractLocked()
    {
        DialogueController.Instance.StartDialogue(DialogueLocked);
    }

    private void ShowScrewOptions()
    {
        var options = new List<DialogueOptionsView.Option>();

        if (HasScrewdriver)
        {
            options.Add(new DialogueOptionsView.Option
            {
                Text = "##VENT_OPTION_OPEN##",
                Action = ScrewAction
            });
        }

        options.Add(new DialogueOptionsView.Option
        {
            Text = "##DO_NOTHING##"
        });

        DialogueOptionsView.Instance.ShowDialogueOptions(new DialogueOptionsView.Settings
        {
            Options = options
        });

        void ScrewAction()
        {
            TransitionView.Instance.StartTransition(new TransitionSettings
            {
                Type = TransitionType.Color,
                Color = Colors.Black,
                Duration = 1.0f,
                DurationHold = 1.0f,
                OnTransition = OnTransition
            });
        }

        void OnTransition()
        {
            SfxUnlock.Play();
            
            GameFlags.SetFlag(FlagUnlocked, 1);
            Data.Game.Save();
            UpdateLocked();
        }
    }

    private void UpdateLocked() => SetLocked(!IsUnlocked);

    private void SetLocked(bool locked)
    {
        Vent.Locked = locked;
        VentCoverLocked.Visible = locked;
        VentCoverUnlocked.Visible = !locked;
    }
}
