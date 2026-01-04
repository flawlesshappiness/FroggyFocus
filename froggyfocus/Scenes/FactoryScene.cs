public partial class FactoryScene : GameScene
{
    protected override void Initialize()
    {
        base.Initialize();
        MainQuestController.Instance.AdvanceManagerQuest(2);
    }
}
