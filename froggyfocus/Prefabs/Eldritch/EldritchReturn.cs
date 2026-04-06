using Godot;

public partial class EldritchReturn : Area3D, IInteractable
{
    [Export]
    public Node3D ReturnNode;

    private const string ReturnId = "##ELDRITCH_RETURN_EYE##";

    public override void _Ready()
    {
        base._Ready();
        DialogueController.Instance.OnDialogueEnded += DialogueEnded;
    }

    public void Interact()
    {
        DialogueController.Instance.StartDialogue(ReturnId);
    }

    private void DialogueEnded(string id)
    {
        if (id == ReturnId)
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
