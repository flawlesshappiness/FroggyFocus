public partial class CaveScene : GameScene
{
    protected override void Initialize()
    {
        base.Initialize();
        MainQuestController.Instance.AdvancePartnerQuest(2);
    }
}
