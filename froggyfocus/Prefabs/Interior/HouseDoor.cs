using Godot;

public partial class HouseDoor : Area3D, IInteractable
{
    [Export]
    public string SceneName;

    [Export]
    public string StartNode;

    [Export]
    public SoundInfo DoorSound;

    public virtual void Interact()
    {
        DoorSound.Play();

        TransitionView.Instance.StartTransition(new TransitionSettings
        {
            Type = TransitionType.Color,
            Color = Colors.Black,
            Duration = 0.5f,
            OnTransition = Transition
        });
    }

    private void Transition()
    {
        Data.Game.CurrentScene = SceneName;
        Data.Game.StartingNode = StartNode;
        Data.Game.Save();

        Scene.Goto(SceneName);
    }
}
