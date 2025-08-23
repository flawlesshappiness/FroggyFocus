public partial class RecordingController : SingletonController
{
    public override string Directory => "Recording";

    public override void _Ready()
    {
        base._Ready();
        RegisterDebugActions();
    }

    private void RegisterDebugActions()
    {
        var category = "RECORDING";

        Debug.RegisterAction(new DebugAction
        {
            Category = category,
            Text = "Goto preview scene",
            Action = GotoPreviewScene
        });

        void GotoPreviewScene(DebugView v)
        {
            v.Close();
            Scene.Goto<BugPreviewScene>();
        }
    }
}
