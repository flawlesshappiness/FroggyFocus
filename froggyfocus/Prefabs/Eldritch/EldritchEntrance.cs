using Godot;

public partial class EldritchEntrance : Area3D
{
    public static EldritchEntrance Instance { get; private set; }

    [Export]
    public bool IsEntrance;

    [Export]
    public MeshInstance3D MeshEntrance;

    [Export]
    public MeshInstance3D MeshExit;

    [Export]
    public CollisionShape3D Collider;

    [Export]
    public AnimationPlayer AnimationPlayer;

    [Export]
    public AudioStreamPlayer SfxWakeUp;

    private bool IsAwake => GameFlags.IsFlag(EldritchFlower.IsAwakeFlag, 1);

    private string DebugId => nameof(EldritchEntrance) + GetInstanceId();

    private bool initialized;
    private bool is_active;

    public override void _Ready()
    {
        base._Ready();

        GameFlagsController.Instance.OnFlagChanged += GameFlagChanged;

        Instance = this;
        BodyEntered += PlayerEntered;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (!initialized)
        {
            initialized = true;
            Initialize();
        }
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        GameFlagsController.Instance.OnFlagChanged -= GameFlagChanged;
        Debug.RemoveActions(DebugId);
    }

    private void Initialize()
    {
        Collider.Disabled = true;
        MeshEntrance.Visible = IsEntrance;
        MeshExit.Visible = !IsEntrance;

        if (!IsEntrance || IsAwake)
        {
            is_active = true;
            Collider.Disabled = false;
            AnimationPlayer.Play("active");
            EldritchTentacle.SetAwakeGlobal(true);
            EldritchEye.SetOpenGlobal(true);
        }
    }

    public void Activate()
    {
        if (is_active) return;
        is_active = true;

        Collider.Disabled = false;
        AnimationPlayer.Play("show");

        EldritchTentacle.SetAwakeGlobal(true);
        EldritchEye.SetOpenGlobal(true);
    }

    private void PlayerEntered(GodotObject go)
    {
        var scene = IsEntrance ? nameof(EldritchScene) : nameof(SwampScene);
        Data.Game.CurrentScene = scene;
        Data.Game.StartingNode = "EldritchStart";
        Data.Game.Save();

        Player.Instance.Disable();

        if (IsEntrance)
        {
            EldritchTransitionView.Instance.StartTransitionEnter();
        }
        else
        {
            EldritchTransitionView.Instance.StartTransitionExit();
        }
    }

    private void GameFlagChanged(string id, int i)
    {
        if (id != EldritchFlower.IsAwakeFlag) return;

        ValidateAwaken();
    }

    public void ValidateAwaken()
    {
        if (is_active) return;

        if (IsAwake)
        {
            Awaken();
        }
    }

    private void Awaken()
    {
        SfxWakeUp.Play();
        Activate();
    }
}
