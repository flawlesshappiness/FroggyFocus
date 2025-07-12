using Godot;

public partial class QuestBoard : Area3D, IInteractable
{
    public void Interact()
    {
        ObjectiveView.Instance.Show();
    }
}
