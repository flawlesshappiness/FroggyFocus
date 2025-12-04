using Godot;
using Godot.Collections;
using System.Linq;

public partial class CrystalDoorway : Area3D, IInteractable
{
    [Export]
    public bool IsEntrance;

    [Export]
    public AnimationPlayer AnimationPlayer;

    [Export]
    public Array<CrystalPillarContainer> Containers;

    private string SceneName => IsEntrance ? nameof(CrystalScene) : nameof(CaveScene);
    private string StartNode => IsEntrance ? "" : "CrystalStart";
    private bool AllContainersValid => Containers.Count == 0 || Containers.All(x => x.IsCompleted);

    private bool is_open;

    public override void _Ready()
    {
        base._Ready();
        InitializeContainers();
        InitializeOpen();
    }

    private void InitializeOpen()
    {
        is_open = AllContainersValid;
        var anim = is_open ? "open" : "closed";
        AnimationPlayer.Play(anim);
    }

    private void InitializeContainers()
    {
        foreach (var container in Containers)
        {
            container.OnCompleted += ContainerCompleted;
        }
    }

    public void Interact()
    {
        if (is_open)
        {
            ChangeScene();
        }
        else
        {
            DialogueController.Instance.StartDialogue("##CRYSTAL_DOORWAY_CLOSED##");
        }
    }

    private void ChangeScene()
    {
        Data.Game.StartingNode = StartNode;
        Data.Game.CurrentScene = SceneName;
        Data.Game.Save();

        TransitionView.Instance.StartTransition(new TransitionSettings
        {
            Type = TransitionType.Color,
            Color = Colors.Black,
            Duration = 1f,
            OnTransition = () =>
            {
                if (IsEntrance)
                {
                    var scene = Scene.Goto<CrystalScene>();
                    scene.StartIntro();
                }
                else
                {
                    Scene.Goto(SceneName);
                }
            }
        });
    }

    public void SetOpen(bool open)
    {
        if (is_open == open) return;
        is_open = open;

        var anim = open ? "opening" : "closed";
        AnimationPlayer.Play(anim);
    }

    private void ContainerCompleted()
    {
        SetOpen(AllContainersValid);

        if (is_open)
        {
            DialogueController.Instance.StartDialogue("##CRYSTAL_CONTAINER_COMPLETED##");
        }
    }

    private void ContainerNotCompleted()
    {
        SetOpen(false);
    }
}
