using Godot;

public partial class Boot : Node
{
    private bool _initialized;

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (!_initialized)
            Initialize();
    }

    public override void _Notification(int what)
    {
        base._Notification(what);

        long id = what;
        if (id is NotificationWMCloseRequest or NotificationCrash or NotificationExitTree)
        {
            Debug.WriteLogsToPersistentData();
        }
    }

    private void Initialize()
    {
        Debug.TraceMethod();

        InitializeTree();
        SingletonController.CreateAll();
        View.CreateAll();
        LoadScene();

        _initialized = true;
    }

    private void InitializeTree()
    {
        Debug.TraceMethod();

        Scene.Tree = GetTree();
        Scene.Root = Scene.Tree.Root;
        Scene.PauseLock.OnLocked += () => Scene.Tree.Paused = true;
        Scene.PauseLock.OnFree += () => Scene.Tree.Paused = false;
    }

    private void LoadScene()
    {
        Debug.TraceMethod();
        Scene.Goto(ApplicationInfo.Instance.StartScene);
    }
}
