using Godot;
using System.Collections.Generic;

public partial class PartnerToolboxInteract : Node3D
{
    [Export]
    public InteractArea InteractArea;

    [Export]
    public HandInInfo HandInInfo;

    [Export]
    public FrogPartnerNpc PartnerNpc;

    private bool HasScrewdriver => GameFlags.IsFlag(ReceptionVent.FlagHasScrewdriver, 1);
    private const string DialogueToolbox = "TOOLBOX_001";
    private const string DialogueTakeScrewdriver = "TOOLBOX_TAKE_SCREWDRIVER_001";

    public override void _Ready()
    {
        base._Ready();
        InteractArea.OnInteract += Interact;
        DialogueController.Instance.OnDialogueEnded += Dialogue_Ended;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        DialogueController.Instance.OnDialogueEnded -= Dialogue_Ended;
    }

    private void Dialogue_Ended(string id)
    {
        if (id == DialogueToolbox)
        {
            ShowOptions();
        }
    }

    private void Interact()
    {
        if (HandInInfo.Data.ClaimCount == 0)
        {
            PartnerNpc.StartToolboxDialogue();
        }
        else
        {
            DialogueController.Instance.StartDialogue(DialogueToolbox);
        }
    }

    private void ShowOptions()
    {
        var options = new List<DialogueOptionsView.Option>();

        if (!HasScrewdriver)
        {
            options.Add(new DialogueOptionsView.Option
            {
                Text = "##TOOLBOX_OPTION_SCREWDRIVER##",
                Action = ActionScrewdriver
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

        void ActionScrewdriver()
        {
            GameFlags.SetFlag(ReceptionVent.FlagHasScrewdriver, 1);
            DialogueController.Instance.StartDialogue(DialogueTakeScrewdriver);
        }
    }
}
