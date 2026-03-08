using Godot;

public partial class TestHandInNpc : Area3D, IInteractable
{
    [Export]
    public HandInInfo HandInInfo;

    public override void _Ready()
    {
        base._Ready();
        DialogueController.Instance.OnNodeEnded += DialogueNodeEnded;
    }

    public void Interact()
    {
        DialogueController.Instance.StartDialogue("##TEST_HAND_IN_001##");
    }

    private void DialogueNodeEnded(string id)
    {
        if (id == "##TEST_HAND_IN_002##")
        {
            var data = HandIn.GetOrCreateData(HandInInfo.Id);
            HandInView.Instance.ShowPopup(data);
        }
    }
}
