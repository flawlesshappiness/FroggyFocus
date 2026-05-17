using Godot;
using System.Collections;

namespace FlawLizArt.FocusEvent;

public partial class FlashEldritchEye : Node3D
{
    [Export]
    public Color FlashColor;

    [Export]
    public AnimationPlayer AnimationPlayer;

    [Export]
    public AnimationPlayer AnimationPlayer_Flash;

    [Export]
    public EldritchEye Eye;

    private FocusCursor Cursor => FocusEvent.Cursor;
    private FocusEvent FocusEvent => Target.FocusEvent;
    private FocusTarget Target { get; set; }

    private Coroutine cr_run;
    private RandomNumberGenerator rng = new();

    public void Initialize(FocusTarget target)
    {
        Target = target;
        FocusEvent.OnEnded += FocusEvent_Ended;
        cr_run = Animate();
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

    private Coroutine Animate()
    {
        return this.StartCoroutine(Cr, "animate");
        IEnumerator Cr()
        {
            yield return AnimationPlayer.PlayAndWaitForAnimation("show");
            yield return new WaitForSeconds(rng.RandfRange(0.5f, 1f));
            Eye.OpenFast();
            AnimationPlayer_Flash.Play("flash");
            yield return new WaitForSeconds(0.25f);
            Flash();
            yield return new WaitForSeconds(1f);
            yield return AnimationPlayer.PlayAndWaitForAnimation("hide");
            QueueFree();
        }
    }

    private void Flash()
    {
        if (!FocusEvent.IsCoveringEyes)
        {
            Cursor.HurtFocusValuePercentage(0.25f);
            Cursor.EndFocusTarget();
            Cursor.GlobalPosition = GlobalPosition.Set(y: Cursor.GlobalPosition.Y);
            FocusEventView.Instance.Flash(1f, FlashColor);
        }
    }
}
