using FlawLizArt.FocusEvent;
using Godot;

public partial class WoodenStab : Node3D
{
    [Export]
    public AnimationPlayer Animation;

    [Export]
    public AudioStreamPlayer SfxWarn;

    private FocusEvent FocusEvent { get; set; }

    public void Initialize(FocusTarget target)
    {
        GlobalPosition = target.GlobalPosition;

        FocusEvent = target.FocusEvent;
        FocusEvent.OnEnded += FocusEvent_Ended;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        FocusEvent.OnEnded -= FocusEvent_Ended;
    }

    private void FocusEvent_Ended(FocusEventResult result)
    {
        Animation.Stop();
        QueueFree();
    }


    public Coroutine AnimateStab()
    {
        return Animation.PlayAndWaitForAnimation("stab");
    }

    public void Warn()
    {
        SfxWarn.Play();
    }
}
