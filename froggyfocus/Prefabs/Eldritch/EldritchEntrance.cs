using Godot;

public partial class EldritchEntrance : Area3D
{
    public static EldritchEntrance Instance { get; private set; }

    [Export]
    public bool AlwaysActive;

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

    private bool initialized;
    private bool is_active;

    public override void _Ready()
    {
        base._Ready();

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

    private void Initialize()
    {
        Collider.Disabled = true;
        MeshEntrance.Visible = IsEntrance;
        MeshExit.Visible = !IsEntrance;

        if (AlwaysActive)
        {
            is_active = true;
            Collider.Disabled = false;
            AnimationPlayer.Play("active");
        }
    }

    public void Activate()
    {
        if (is_active) return;
        is_active = true;

        Collider.Disabled = false;
        AnimationPlayer.Play("show");
    }

    private void PlayerEntered(GodotObject go)
    {
        var scene = IsEntrance ? nameof(EldritchScene) : nameof(SwampScene);
        Data.Game.CurrentScene = scene;
        Data.Game.StartingNode = "EldritchStart";
        Data.Game.Save();

        Player.Instance.Disable();

        EldritchTransitionView.Instance.StartTransition(() =>
        {
            Scene.Goto(Data.Game.CurrentScene);
        });
    }
}
