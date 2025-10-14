using FlawLizArt.Animation.StateMachine;
using Godot;
using System;

public enum EldritchEyeStartState
{
    Closed, Open, Pout
}

public partial class EldritchEye : Node3D
{
    [Export]
    public EldritchEyeStartState StartState;

    [Export]
    public Vector2 BlinkDelay;

    [Export]
    public AnimationStateMachine Animation;

    private static event Action<bool> OnOpenStateChanged;

    private float time_blink;
    private RandomNumberGenerator rng = new();
    private BoolParameter param_open = new BoolParameter("open", false);
    private TriggerParameter param_blink = new TriggerParameter("blink");

    public override void _Ready()
    {
        base._Ready();
        InitializeAnimations();

        OnOpenStateChanged += OpenStateChanged;
    }

    public override void _ExitTree()
    {
        base._ExitTree();

        OnOpenStateChanged -= OpenStateChanged;
    }

    private void InitializeAnimations()
    {
        var idle_closed = Animation.CreateAnimation("Armature|idle_closed", true);
        var idle_open = Animation.CreateAnimation("Armature|idle_open", true);
        var idle_pout = Animation.CreateAnimation("Armature|idle_pout", true);
        var open = Animation.CreateAnimation("Armature|open", false);
        var close = Animation.CreateAnimation("Armature|close", false);
        var blink = Animation.CreateAnimation("Armature|blink", false);

        Animation.Connect(idle_closed, open, param_open.WhenTrue());
        Animation.Connect(open, idle_open);

        Animation.Connect(idle_open, close, param_open.WhenFalse());
        Animation.Connect(close, idle_closed);

        Animation.Connect(idle_open, blink, param_blink.WhenTriggered());
        Animation.Connect(blink, idle_open);
        Animation.Connect(blink, close, param_open.WhenFalse());

        var start = StartState switch
        {
            EldritchEyeStartState.Closed => idle_closed,
            EldritchEyeStartState.Open => idle_open,
            EldritchEyeStartState.Pout => idle_pout,
            _ => idle_closed
        };

        Animation.Start(start.Node);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (!param_open.Value) return;
        if (GameTime.Time < time_blink) return;

        ResetBlinkTime();
        Blink();
    }

    private void ResetBlinkTime()
    {
        time_blink = GameTime.Time + BlinkDelay.Range(rng.Randf());
    }

    public void Blink()
    {
        param_blink.Trigger();
    }

    public void SetOpen(bool open)
    {
        param_open.Set(open);
        ResetBlinkTime();
    }

    public static void SetOpenGlobal(bool open)
    {
        OnOpenStateChanged?.Invoke(open);
    }

    private void OpenStateChanged(bool open)
    {
        SetOpen(open);
    }
}
