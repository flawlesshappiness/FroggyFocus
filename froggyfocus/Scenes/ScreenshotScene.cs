using Godot;

public partial class ScreenshotScene : GameScene
{
    [Export]
    public ScreenshotSceneSetup LogoSetup;

    [Export]
    public ScreenshotSceneSetup BecomeFroggySetup;

    [Export]
    public ScreenshotSceneSetup MeetFriendsSetup;

    [Export]
    public ScreenshotSceneSetup CatchBugsSetup;

    [Export]
    public ScreenshotSceneSetup HorizontalCapsuleSetup;

    [Export]
    public ScreenshotSceneSetup SmallCapsuleSetup;

    [Export]
    public ScreenshotSceneSetup VerticalCapsuleSetup;

    [Export]
    public ScreenshotSceneSetup LibraryHeroSetup;

    [Export]
    public ScreenshotSceneSetup SkyboxBackgroundSetup;

    [Export]
    public ScreenshotSceneSetup IconSetup;

    private string DebugId => nameof(ScreenshotScene) + GetInstanceId();

    public override void _Ready()
    {
        base._Ready();
        RegisterDebugActions();
        HideAllSetups();
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        Debug.RemoveActions(DebugId);
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

            v.ContentSearch.AddItem("Logo", () => ShowSetup(v, LogoSetup));
            v.ContentSearch.AddItem("Become Froggy", () => ShowSetup(v, BecomeFroggySetup));
            v.ContentSearch.AddItem("Meet Friends", () => ShowSetup(v, MeetFriendsSetup));
            v.ContentSearch.AddItem("Catch Bugs", () => ShowSetup(v, CatchBugsSetup));
            v.ContentSearch.AddItem("Capsule Horizontal", () => ShowSetup(v, HorizontalCapsuleSetup));
            v.ContentSearch.AddItem("Capsule Small", () => ShowSetup(v, SmallCapsuleSetup));
            v.ContentSearch.AddItem("Capsule Vertical", () => ShowSetup(v, VerticalCapsuleSetup));
            v.ContentSearch.AddItem("Library Hero", () => ShowSetup(v, LibraryHeroSetup));
            v.ContentSearch.AddItem("Skybox Background", () => ShowSetup(v, SkyboxBackgroundSetup));
            v.ContentSearch.AddItem("Icon", () => ShowSetup(v, IconSetup));

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
        LogoSetup.Hide();
        BecomeFroggySetup.Hide();
        MeetFriendsSetup.Hide();
        CatchBugsSetup.Hide();
        HorizontalCapsuleSetup.Hide();
        SmallCapsuleSetup.Hide();
        VerticalCapsuleSetup.Hide();
        LibraryHeroSetup.Hide();
        SkyboxBackgroundSetup.Hide();
        IconSetup.Hide();
    }
}
