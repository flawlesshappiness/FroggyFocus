public partial class EldritchScene : GameScene
{
    public override void _Ready()
    {
        base._Ready();
        FloatingPlatformController.Instance.Start();
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        FloatingPlatformController.Instance.Stop();
    }
}
