using Godot;

public partial class QuestBoard : Area3D, IInteractable
{
    [Export]
    public Node3D Exclamation;

    public override void _Ready()
    {
        base._Ready();
        ObjectiveController.Instance.OnObjectiveComplete += ObjectiveComplete;
        Exclamation.Visible = ObjectiveController.Instance.IsAnyObjectiveComplete();
    }

    public void Interact()
    {
        ObjectiveView.Instance.Show();
        Exclamation.Hide();
    }

    private void ObjectiveComplete()
    {
        Exclamation.Show();
    }
}
