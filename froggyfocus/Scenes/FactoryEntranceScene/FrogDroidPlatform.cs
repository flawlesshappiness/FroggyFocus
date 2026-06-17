using FlawLizArt.FocusEvent;
using Godot;
using System.Linq;

public partial class FrogDroidPlatform : Node3D
{
    [Export]
    public FocusEventInfo FocusEventInfo;

    [Export]
    public CuteFrogCharacter Character;

    [Export]
    public InteractArea InteractArea;

    private const string DialoguePeer = "ROBOT_PEER";

    public override void _Ready()
    {
        base._Ready();
        InitializeAppearance();
        InteractArea.OnInteract += Interact;
        DialogueController.Instance.OnDialogueEnded += Dialogue_Ended;
        FocusEventController.Instance.OnFocusEventEnded += FocusEvent_Ended;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        DialogueController.Instance.OnDialogueEnded -= Dialogue_Ended;
        FocusEventController.Instance.OnFocusEventEnded -= FocusEvent_Ended;
    }

    private void InitializeAppearance()
    {
        Character.ClearAppearance();
        Character.SetBodyBase(Colors.DarkGray);
        Character.SetBodyTop(ItemType.BodyTop_Robot, Colors.Black);
        Character.SetBodyPattern(ItemType.BodyPattern_None, Colors.Transparent);
        Character.SetBodyEye(ItemType.BodyEye_Robot, Colors.Red);
    }

    private void Interact()
    {
        DialogueController.Instance.StartDialogue(DialoguePeer);
    }

    private void Dialogue_Ended(string id)
    {
        if (id == DialoguePeer)
        {
            DialogueOptionsView.Instance.ShowDialogueOptions(new DialogueOptionsView.Settings
            {
                Options = new System.Collections.Generic.List<DialogueOptionsView.Option>
                {
                    new DialogueOptionsView.Option
                    {
                        Text = "##ROBOT_OPTION_CATCH##",
                        Action = CatchAction
                    },
                    new DialogueOptionsView.Option
                    {
                        Text = "##DO_NOTHING##",
                    }
                }
            });
        }

        void CatchAction()
        {
            GameScene.Instance.FocusEvent.StartEvent(new FocusEvent.Settings
            {
                Id = FocusEventInfo.Id,
                OverrideTargetRarity = 5,
            });
        }
    }

    private void FocusEvent_Ended(FocusEventResult result)
    {
        if (result.FocusEvent.Targets.All(x => x.IsCaught && x.Lives == 0))
        {
            EndTextView.Instance.Show();
        }
    }
}
