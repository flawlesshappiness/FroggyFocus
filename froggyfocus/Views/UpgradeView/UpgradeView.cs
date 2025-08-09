using Godot;

public partial class UpgradeView : PanelView
{
    public static UpgradeView Instance => Get<UpgradeView>();
    protected override bool IgnoreCreate => false;

    [Export]
    public Button BackButton;

    [Export]
    public UpgradeContainer Upgrades;

    public override void _Ready()
    {
        base._Ready();
        RegisterDebugActions();

        BackButton.Pressed += BackButton_Pressed;
    }

    private void RegisterDebugActions()
    {
        var category = "UPGRADES";

        Debug.RegisterAction(new DebugAction
        {
            Category = category,
            Text = "Show view",
            Action = v => { v.Close(); Show(); }
        });

        Debug.RegisterAction(new DebugAction
        {
            Category = category,
            Text = "Hide view",
            Action = v => { v.Close(); Close(); }
        });
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (Input.IsActionJustReleased("ui_cancel") && IsVisibleInTree())
        {
            Close();
        }
    }

    protected override void GrabFocusAfterOpen()
    {
        base.GrabFocusAfterOpen();
        Upgrades.GetFirstButton().GrabFocus();
    }

    private void BackButton_Pressed()
    {
        Close();
    }
}
