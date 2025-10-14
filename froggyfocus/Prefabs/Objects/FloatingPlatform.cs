using FlawLizArt.Animation.StateMachine;
using Godot;

public partial class FloatingPlatform : Node3D
{
    [Export]
    public int TickUp;

    [Export]
    public int TickWarn;

    [Export]
    public int TickDown;

    [Export]
    public int TickOffset;

    [Export]
    public AnimationStateMachine Animation;

    private int TickMax => TickUp + TickWarn + TickDown;

    private BoolParameter param_warn = new BoolParameter("warn", false);
    private BoolParameter param_up = new BoolParameter("up", true);

    public override void _Ready()
    {
        base._Ready();
        FloatingPlatformController.Instance.OnTick += Tick;
        InitializeAnimations();
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        FloatingPlatformController.Instance.OnTick -= Tick;
    }

    private void InitializeAnimations()
    {
        var idle = Animation.CreateAnimation("idle", true);
        var show = Animation.CreateAnimation("show", false);
        var hide = Animation.CreateAnimation("hide", false);
        var shake = Animation.CreateAnimation("shake", true);

        Animation.Connect(hide, show, param_up.WhenTrue());
        Animation.Connect(show, idle);
        Animation.Connect(idle, hide, param_up.WhenFalse());
        Animation.Connect(idle, shake, param_warn.WhenTrue());
        Animation.Connect(shake, hide, param_up.WhenFalse());

        Animation.Start(show.Node);
    }

    private void Tick(int tick)
    {
        var t = (tick + TickOffset) % TickMax;

        if (t < TickUp)
        {
            param_up.Set(true);
            param_warn.Set(false);
        }
        else if (t < TickUp + TickWarn)
        {
            param_warn.Set(true);
        }
        else
        {
            param_up.Set(false);
            param_warn.Set(false);
        }
    }
}
