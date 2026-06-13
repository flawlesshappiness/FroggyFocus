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
    public Array<CollisionShape3D> Colliders;

    private const string DialogueBlocked = "RECEPTIONIST_BLOCK";

    public override void _Ready()
    {
        base._Ready();
        ReceptionTriggerArea.OnNodeEntered += ReceptionTriggerArea_Entered;
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

    private void UpdateColliders()
    {
        var enabled = HandInInfo.Data.ClaimCount == 0;
        Colliders.ForEach(x => x.Disabled = !enabled);
    }
}
