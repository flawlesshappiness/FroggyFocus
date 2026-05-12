public partial class EldritchScene : GameScene
{
    public override void _Ready()
    {
        base._Ready();
        FloatingPlatformController.Instance.Start();
    }

    protected override void Initialize()
    {
        base.Initialize();
        MainQuestController.Instance.AdvanceScientistQuest(2);
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        FloatingPlatformController.Instance.Stop();
    }
}
