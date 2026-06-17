using Godot;
using System.Collections;

namespace FlawLizArt.FocusEvent;

public partial class RobotEyes : Node3D
{
    [Export]
    public Color FlashColor;

    [Export]
    public AnimationPlayer AnimationPlayer;

    [Export]
    public AudioStreamPlayer SfxHit;

    [Export]
    public AudioStreamPlayer SfxBlock;

    private FocusTarget Target { get; set; }
    private FocusEvent FocusEvent => Target.FocusEvent;
    private FocusCursor Cursor => FocusEvent.Cursor;

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
        Destroy();
    }

    private void Destroy()
    {
        Coroutine.Stop(cr_run);
        QueueFree();
    }

    public void Run()
    {
        GlobalPosition = FocusEvent.GlobalPosition;

        cr_run = this.StartCoroutine(Cr, "run");
        IEnumerator Cr()
        {
            AnimationPlayer.Play("show");
            yield return new WaitForSeconds(1f);
            AnimationPlayer.Play("hide");

            if (!FocusEvent.IsCoveringEyes)
            {
                Cursor.HurtFocusValuePercentage(0.1f);
                Cursor.EndFocusTarget();
                FocusEventView.Instance.Flash(1f, FlashColor);
                SfxHit.Play();
            }
            else
            {
                FocusEventView.Instance.Flash(1f, FlashColor.SetA(0.25f));
                SfxBlock.Play();
            }

            yield return new WaitForSeconds(1f);

            Destroy();
        }
    }
}
