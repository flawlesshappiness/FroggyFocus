using Godot;

public partial class ReceptionTrigger : Node3D
{
    [Export]
    public TriggerArea3D ReceptionTriggerArea;

    [Export]
    public ReceptionistNpc Receptionist;

    [Export]
    public Camera3D Camera;

    [Export]
    public Marker3D ExitMarker;

    private const string DialogueBlocked = "RECEPTIONIST_BLOCK";

    public override void _Ready()
    {
        base._Ready();
        ReceptionTriggerArea.OnNodeEntered += ReceptionTriggerArea_Entered;
        DialogueController.Instance.OnDialogueEnded += Dialogue_Ended;
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
            Player.Instance.TeleportToNode(ExitMarker);
            DialogueController.Instance.StartDialogue(DialogueBlocked);
            Receptionist.Looking = true;
            Camera.Current = true;
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
    }
}
