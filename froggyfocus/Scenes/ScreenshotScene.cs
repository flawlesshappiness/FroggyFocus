using System.Collections.Generic;

public partial class ScreenshotScene : GameScene
{
    private string DebugId => nameof(ScreenshotScene) + GetInstanceId();

    private List<ScreenshotSceneSetup> screenshot_setups = new();

    public override void _Ready()
    {
        base._Ready();
        InitializeSetups();
        RegisterDebugActions();
        HideAllSetups();
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        Debug.RemoveActions(DebugId);
    }

    private void InitializeSetups()
    {
        screenshot_setups = this.GetNodesInChildren<ScreenshotSceneSetup>();
    }

    private void RegisterDebugActions()
    {
        var category = "SCREENSHOT SCENE";

        Debug.RegisterAction(new DebugAction
        {
            Id = DebugId,
            Category = category,
            Text = "Hide setups",
            Action = HideSetups,
        });

        void HideSetups(DebugView v)
        {
            HideAllSetups();
            Player.Instance.SetCameraTarget();
            v.Close();
        }

        Debug.RegisterAction(new DebugAction
        {
            Id = DebugId,
            Category = category,
            Text = "Select setup",
            Action = ListSetups,
        });

        void ListSetups(DebugView v)
        {
            v.SetContent_Search();

            foreach (var setup in screenshot_setups)
            {
                v.ContentSearch.AddItem(setup.Id, () => ShowSetup(v, setup));
            }

            v.ContentSearch.UpdateButtons();
        }

        void ShowSetup(DebugView v, ScreenshotSceneSetup setup)
        {
            HideAllSetups();
            setup.ShowSetup();
        }
    }

    private void HideAllSetups()
    {
        screenshot_setups.ForEach(x => x.Hide());
    }
}
