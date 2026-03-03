using FlawLizArt.FocusEvent;
using Godot;
using System.Collections;

public partial class FlashMushroom : Node3D
{
    [Export]
    public Color FlashColor;

    [Export]
    public AnimationPlayer Animation_Mushroom;

    [Export]
    public AnimationPlayer Animation_Flash;

    [Export]
    public PackedScene ExplodeEffectPrefab;

    private FocusCursor Cursor => FocusEvent.Cursor;
    private FocusEvent FocusEvent { get; set; }
    private FocusTarget Target { get; set; }

    private RandomNumberGenerator rng = new();
    private Coroutine cr_run;

    public void Initialize(FocusTarget target)
    {
        Target = target;

        FocusEvent = target.FocusEvent;
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
        GlobalPosition = Target.GetNextPosition();

        var duration_idle_min = Mathf.Lerp(2f, 1f, Target.Difficulty);
        var duration_idle_max = Mathf.Lerp(4f, 2f, Target.Difficulty);
        var duration_idle = rng.RandfRange(duration_idle_min, duration_idle_max);

        var duration_telegraph_min = Mathf.Lerp(0.2f, 0.1f, Target.Difficulty);
        var duration_telegraph_max = Mathf.Lerp(0.5f, 0.3f, Target.Difficulty);
        var duration_telegraph = rng.RandfRange(duration_telegraph_min, duration_telegraph_max);

        cr_run = this.StartCoroutine(Cr, "run");
        IEnumerator Cr()
        {
            yield return Animation_Mushroom.PlayAndWaitForAnimation("show");
            yield return new WaitForSeconds(duration_idle);
            Animation_Mushroom.Play("idle");
            yield return new WaitForSeconds(duration_telegraph);
            yield return Animation_Flash.PlayAndWaitForAnimation("telegraph");
            yield return new WaitForSeconds(0.2f);
            yield return Animation_Mushroom.PlayAndWaitForAnimation("explode");
            Animation_Flash.Play("flash");
            Flash();
        }
    }

    private void Flash()
    {
        if (!FocusEvent.IsCoveringEyes)
        {
            Cursor.HurtFocusValuePercentage(0.1f);
            Cursor.EndFocusTarget();
            FocusEventView.Instance.Flash(1f, FlashColor);
        }


        var ps = ExplodeEffectPrefab.Instantiate<ParticleEffectGroup>();
        ps.SetParent(FocusEvent);
        ps.GlobalPosition = Target.GlobalPosition;
    }
}
