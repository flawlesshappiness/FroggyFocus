using Godot;

public partial class TutorialSign : Area3D, IInteractable
{
    [Export]
    public Type SignType;

    public enum Type
    {
        Jump,
        Bugs,
        Shield,
        Forfeit
    }

    public void Interact()
    {
        TutorialView.Instance.ShowSign(SignType);
    }
}
