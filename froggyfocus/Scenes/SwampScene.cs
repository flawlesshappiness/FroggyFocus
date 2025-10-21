public partial class SwampScene : GameScene
{
    public override void _Ready()
    {
        base._Ready();
        RegisterDebugActions();
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        Debug.RemoveActions(GetInstanceId().ToString());
    }

    private void RegisterDebugActions()
    {
        var category_eldritch = "ELDRITCH";

        Debug.RegisterAction(new DebugAction
        {
            Id = GetInstanceId().ToString(),
            Category = category_eldritch,
            Text = "Wake up",
            Action = EldritchWakeUp
        });

        Debug.RegisterAction(new DebugAction
        {
            Id = GetInstanceId().ToString(),
            Category = category_eldritch,
            Text = "Sleep",
            Action = EldritchSleep
        });

        void EldritchWakeUp(DebugView v)
        {
            EldritchTentacle.SetAwakeGlobal(true);
            EldritchEye.SetOpenGlobal(true);
            EldritchEntrance.Instance.Activate();
            v.Close();
        }

        void EldritchSleep(DebugView v)
        {
            EldritchTentacle.SetAwakeGlobal(false);
            EldritchEye.SetOpenGlobal(false);
            v.Close();
        }
    }
}
