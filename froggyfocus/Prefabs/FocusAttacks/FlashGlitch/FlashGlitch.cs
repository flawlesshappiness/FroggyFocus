using FlawLizArt.FocusEvent;
using Godot;
using System.Collections;

public partial class FlashGlitch : Node3D
{
    [Export]
    public AnimationPlayer AnimationPlayer;

    public static FlashGlitch CurrentFlasher;

    private FocusCursor Cursor => FocusEvent.Cursor;
    private FocusEvent FocusEvent => Target.FocusEvent;
    private FocusTarget Target { get; set; }

    private Coroutine cr_run;

    public void Initialize(FocusTarget target)
    {
        Target = target;
        FocusEvent.OnEnded += FocusEvent_Ended;

        Run();
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        FocusEvent.OnEnded -= FocusEvent_Ended;
    }

    private void FocusEvent_Ended(FocusEventResult result)
    {
        Coroutine.Stop(cr_run);
        QueueFree();
    }

    private void Run()
    {
        cr_run = this.StartCoroutine(Cr, "run");
        IEnumerator Cr()
        {
            yield return AnimationPlayer.PlayAndWaitForAnimation("run");
            yield return Flash();
            CurrentFlasher = null;
        }
    }

    private IEnumerator Flash()
    {
        if (!FocusEvent.IsCoveringEyes)
        {
            Cursor.HurtFocusValuePercentage(0.25f);
            Cursor.EndFocusTarget();
            yield return FocusEventView.Instance.FlashGlitch(1f);
        }

        yield return null;
    }
}
