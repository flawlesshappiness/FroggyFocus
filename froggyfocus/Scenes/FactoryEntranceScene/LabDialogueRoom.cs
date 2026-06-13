using Godot;

public partial class LabDialogueRoom : Node3D
{
    [Export]
    public string DialogueId;

    [Export]
    public VentDoor Vent;

    [Export]
    public Camera3D Camera;

    [Export]
    public Node3D LabRoom;

    public override void _Ready()
    {
        base._Ready();
        Vent.Locked = true;
        Vent.OnInteractLocked += Vent_Interact;
        DialogueController.Instance.OnDialogueEnded += Dialogue_Ended;

        LabRoom.Hide();
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        DialogueController.Instance.OnDialogueEnded -= Dialogue_Ended;
    }

    private void Vent_Interact()
    {
        TransitionView.Instance.StartTransition(new TransitionSettings
        {
            Type = TransitionType.Color,
            Color = Colors.Black,
            Duration = 0.5f,
            OnTransition = StartDialogue
        });
    }

    private void StartDialogue()
    {
        Camera.Current = true;
        LabRoom.Show();
        DialogueController.Instance.StartDialogue(DialogueId);
        GameView.Instance.AnimateVignetteShow(0f);
    }

    private void End()
    {
        LabRoom.Hide();
        Player.Instance.SetCameraTarget();
        GameView.Instance.AnimateVignetteHide(0f);
    }

    private void Dialogue_Ended(string id)
    {
        if (id == DialogueId)
        {
            TransitionView.Instance.StartTransition(new TransitionSettings
            {
                Type = TransitionType.Color,
                Color = Colors.Black,
                Duration = 0.5f,
                OnTransition = End
            });
        }
    }
}
