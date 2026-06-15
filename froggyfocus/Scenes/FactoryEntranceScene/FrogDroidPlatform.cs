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
        Character.SetBodyPattern(ItemType.BodyPattern_None, Colors.Red);
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
