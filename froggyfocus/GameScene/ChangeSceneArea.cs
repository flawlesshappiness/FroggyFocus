using Godot;

[GlobalClass]
public partial class ChangeSceneArea : Area3D
{
    [Export]
    public string SceneName;

    [Export]
    public string StartNode;

    public override void _Ready()
    {
        base._Ready();
        BodyEntered += _BodyEntered;
    }

    private void _BodyEntered(GodotObject go)
    {
        TransitionView.Instance.StartTransition(new TransitionSettings
        {
            Type = TransitionType.Color,
            Color = Colors.Black,
            Duration = 1f,
            OnTransition = ChangeScene
        });
    }

    private void ChangeScene()
    {
        Data.Game.CurrentScene = SceneName;
        Data.Game.StartingNode = StartNode;
        Data.Game.Save();

        Scene.Goto(SceneName);
    }
}
