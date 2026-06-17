using Godot;
using Godot.Collections;

public partial class ReceptionTrigger : Node3D
{
    [Export]
    public HandInInfo HandInInfo;

    [Export]
    public TriggerArea3D ReceptionTriggerArea;

    [Export]
    public ReceptionistNpc Receptionist;

    [Export]
    public Camera3D Camera;

    [Export]
    public Marker3D ExitMarker;

    [Export]
    public AudioStreamPlayer SfxSpeak;

    [Export]
    public Array<CollisionShape3D> Colliders;

    private bool active_dialogue;
    private const string DialogueBlocked = "RECEPTIONIST_BLOCK";

    public override void _Ready()
    {
        base._Ready();
        ReceptionTriggerArea.OnNodeEntered += ReceptionTriggerArea_Entered;
        DialogueController.Instance.OnEntryStarted += DialogueEntry_Ended;
        DialogueController.Instance.OnDialogueEnded += Dialogue_Ended;
        UpdateColliders();
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        DialogueController.Instance.OnDialogueEnded -= Dialogue_Ended;
    }

    private void ReceptionTriggerArea_Entered(Node3D node)
    {
        TransitionView.Instance.StartTransition(new TransitionSettings
        {
            Type = TransitionType.Color,
            Color = Colors.Black,
            Duration = 0.5f,
            OnTransition = OnTransition
        });

        void OnTransition()
        {
            active_dialogue = true;
            Player.Instance.TeleportToNode(ExitMarker);
            DialogueController.Instance.StartDialogue(DialogueBlocked);
            Receptionist.Looking = true;
            Camera.Current = true;
        }
    }

    private void DialogueEntry_Ended(string id)
    {
        if (active_dialogue)
        {
            SfxSpeak?.Play();
        }
    }

    private void Dialogue_Ended(string id)
    {
        if (id == DialogueBlocked)
        {
            Receptionist.Looking = false;

            TransitionView.Instance.StartTransition(new TransitionSettings
            {
                Type = TransitionType.Color,
                Color = Colors.Black,
                Duration = 0.5f,
                OnTransition = OnTransition
            });

            void OnTransition()
            {
                Player.Instance.SetCameraTarget();
            }
        }

        active_dialogue = false;
    }

    private void UpdateColliders()
    {
        var enabled = HandInInfo.Data.ClaimCount == 0;
        Colliders.ForEach(x => x.Disabled = !enabled);
    }
}
