using Godot;
using System.Collections;

namespace FlawLizArt.FocusEvent;

public partial class MossBush : Node3D
{
    [Export]
    public AnimationPlayer Animation;

    private FocusEvent FocusEvent { get; set; }
    private bool Initializded { get; set; }
    public bool Triggered { get; private set; }
    private bool Started { get; set; }
    private bool Ended { get; set; }

    public void Initialize(FocusEvent focus_event)
    {
        FocusEvent = focus_event;
        FocusEvent.OnEnded += FocusEvent_Ended;

        Initializded = true;
        Started = false;
        Triggered = false;
        Ended = false;

        Start();
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        FocusEvent.OnEnded -= FocusEvent_Ended;
    }

    private void FocusEvent_Ended(FocusEventResult result)
    {
        QueueFree();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (!Initializded) return;
        if (!Started) return;
        if (Ended) return;

        var dist = GlobalPosition.DistanceTo(FocusEvent.Cursor.GlobalPosition);
        var length = FocusEvent.Cursor.Radius + 0.5f;
        if (dist < length)
        {
            Trigger();
        }
    }

    private void Start()
    {
        this.StartCoroutine(Cr, "start");
        IEnumerator Cr()
        {
            yield return AnimateShow();
            Started = true;
        }
    }

    private void Trigger()
    {
        Ended = true;

        this.StartCoroutine(Cr, "hide_and_destroy");
        IEnumerator Cr()
        {
            yield return AnimateShake();
            Triggered = true;
            yield return AnimateHide();
            QueueFree();
        }
    }

    public Coroutine AnimateShow()
    {
        return Animation.PlayAndWaitForAnimation("show");
    }

    public Coroutine AnimateHide()
    {
        return Animation.PlayAndWaitForAnimation("hide");
    }

    public Coroutine AnimateShake()
    {
        return Animation.PlayAndWaitForAnimation("shake");
    }
}
