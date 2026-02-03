using Godot;

public partial class CrystalDoorway : Area3D, IInteractable
{
    [Export]
    public bool IsEntrance;

    private string SceneName => IsEntrance ? nameof(CrystalScene) : nameof(CaveScene);
    private string StartNode => IsEntrance ? "" : "CrystalStart";
    private bool IsOpen => !IsEntrance;

    public void Interact()
    {
        if (IsOpen)
        {
            ChangeScene();
        }
        else
        {
            DialogueController.Instance.StartDialogue("##CRYSTAL_DOOR_LOCKED##");
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
}
