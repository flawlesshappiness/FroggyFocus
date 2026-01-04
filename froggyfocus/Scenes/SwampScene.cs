public partial class SwampScene : GameScene
{
    private string DebugId => $"{nameof(SwampScene)}{GetInstanceId()}";

    public override void _Ready()
    {
        base._Ready();
        RegisterDebugActions();
    }

    protected override void Initialize()
    {
        base.Initialize();

        if (GameFlags.IsFlag(LetterScene.INTRO_LETTERS_ID, 0))
        {
            GameFlags.SetFlag(LetterScene.INTRO_LETTERS_ID, 1);
            GameView.Instance.TriggerQuestAdvancedNotification();
        }
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
