public partial class SwampScene : GameScene
{
    private string DebugId => $"{nameof(SwampScene)}{GetInstanceId()}";

    public override void _Ready()
    {
        base._Ready();
        RegisterDebugActions();
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        Debug.RemoveActions(DebugId);
    }

    private void RegisterDebugActions()
    {
        var category = "SWAMP SCENE";
    }
}
