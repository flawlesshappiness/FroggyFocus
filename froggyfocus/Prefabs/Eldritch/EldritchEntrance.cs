using Godot;
using Godot.Collections;
using System.Linq;

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

    [Export]
    public AudioStreamPlayer SfxWakeUp;

    [Export]
    public Array<EldritchFlower> Flowers;

    private string DebugId => nameof(EldritchEntrance) + GetInstanceId();

    private bool initialized;
    private bool is_active;

    public override void _Ready()
    {
        base._Ready();

        Instance = this;
        BodyEntered += PlayerEntered;

        foreach (var flower in Flowers)
        {
            flower.OnCompleted += FlowerCompleted;
        }

        RegisterDebugActions();
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

    private void RegisterDebugActions()
    {
        var category = "ELDRITCH ENTRANCE";

        if (!AlwaysActive)
        {
            Debug.RegisterAction(new DebugAction
            {
                Id = DebugId,
                Category = category,
                Text = "Open",
                Action = v => SetOpen(v, true)
            });

            Debug.RegisterAction(new DebugAction
            {
                Id = DebugId,
                Category = category,
                Text = "Close",
                Action = v => SetOpen(v, false)
            });
        }

        void SetOpen(DebugView v, bool open)
        {
            Flowers.ForEach(x => x.DebugSetCompleted(open));

            if (open)
            {
                FlowerCompleted();
            }

            Data.Game.Save();
            v.Close();
        }
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        Debug.RemoveActions(DebugId);
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
        else if (ValidateFlowers())
        {
            Activate();
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

    private bool ValidateFlowers()
    {
        return Flowers.All(x => x.IsCompleted);
    }

    private void FlowerCompleted()
    {
        if (AlwaysActive) return;
        if (is_active) return;

        if (ValidateFlowers())
        {
            Awaken();
        }
        else
        {
            DialogueController.Instance.StartDialogue("##ELDRITCH_FLOWER_MORE##");
        }
    }

    private void Awaken()
    {
        SfxWakeUp.Play();
        Activate();
        DialogueController.Instance.StartDialogue("##ELDRITCH_FLOWER_RUMBLE##");
    }
}
