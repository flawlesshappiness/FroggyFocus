using Godot;

public partial class EldritchReturn : Area3D, IInteractable
{
    [Export]
    public Node3D ReturnNode;

    private const string DialogueReturn = "ELDRITCH_RETURN_EYE";

    public override void _Ready()
    {
        base._Ready();
        DialogueController.Instance.OnEntryEnded += DialogueEntry_Ended;
    }

    public void Interact()
    {
        DialogueController.Instance.StartDialogue(DialogueReturn);
    }

    private void DialogueEntry_Ended(string id)
    {
        if (id == DialogueReturn)
        {
            GameView.Instance.ShowPopup("##ELDRITCH_RETURN_TEXT##", "##OK##", "##CANCEL##", () =>
            {
                Return();
            },
            () =>
            {

            });
        }
    }

    private void Return()
    {
        EldritchTransitionView.Instance.StartTransitionShort(() =>
        {
            Player.Instance.GlobalPosition = ReturnNode.GlobalPosition;
            Player.Instance.ThirdPersonCamera.SnapToPosition();
        });
    }
}
