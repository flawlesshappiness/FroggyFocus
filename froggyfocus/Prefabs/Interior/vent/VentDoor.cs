using Godot;
using System;

public partial class VentDoor : Node3D
{
    [Export]
    public string SceneName;

    [Export]
    public string NodeName;

    [Export]
    public InteractArea InteractArea;

    public bool Locked { get; set; }

    public event Action OnInteractLocked;
    public event Action OnTransition;

    public override void _Ready()
    {
        base._Ready();
        InteractArea.OnInteract += Interact;
    }

    private void Interact()
    {
        if (Locked)
        {
            OnInteractLocked?.Invoke();
        }
        else
        {
            Transition();
        }
    }

    private void Transition()
    {
        TransitionView.Instance.StartTransition(new TransitionSettings
        {
            Type = TransitionType.Color,
            Color = Colors.Black,
            Duration = 1.0f,
            OnTransition = Transition
        });

        void Transition()
        {
            if (string.IsNullOrEmpty(SceneName))
            {
                var node = GameScene.Instance.FindChild(NodeName) as Node3D;
                if (IsInstanceValid(node))
                {
                    Player.Instance.TeleportToNode(node);
                    OnTransition?.Invoke();
                }
            }
            else
            {
                Data.Game.CurrentScene = SceneName;
                Data.Game.StartingNode = NodeName;
                Data.Game.Save();
                Scene.Goto(SceneName);
            }
        }
    }
}
