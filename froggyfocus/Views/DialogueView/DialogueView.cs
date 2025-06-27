using Godot;

public partial class DialogueView : View
{
    [Export]
    public AnimatedPanel AnimatedPanel_Dialogue;

    public override void _Ready()
    {
        base._Ready();
        RegisterDebugActions();
    }

    private void RegisterDebugActions()
    {
        var category = nameof(DialogueView);

        Debug.RegisterAction(new DebugAction
        {
            Category = category,
            Text = "Show test",
            Action = DebugShowTest
        });

        void DebugShowTest(DebugView v)
        {
            v.Close();
            Show();
            AnimatedPanel_Dialogue.AnimatePopShow();
        }
    }
}
