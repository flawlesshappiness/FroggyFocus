using FlawLizArt.FocusEvent;
using Godot;
using System.Collections;

public partial class CursorCrystal : Node3D
{
    [Export]
    public AnimationPlayer AnimationPlayer;

    [Export]
    public Node3D SizeNode;

    [Export]
    public AudioStreamPlayer SfxCharge;

    [Export]
    public AudioStreamPlayer SfxHit;

    [Export]
    public AudioStreamPlayer SfxBreak;

    [Export]
    public EffectGroupSpawner HitEffect;

    [Export]
    public EffectGroupSpawner ChargeEffect;

    [Export]
    public EffectGroupSpawner BreakEffect;

    private bool Completed => HitCount <= 0;

    private int HitCount { get; set; }
    private int HitCountMax { get; set; }
    private FocusCursor Cursor { get; set; }
    private FocusEvent FocusEvent { get; set; }

    private RandomNumberGenerator rng = new();
    private bool pressed;
    private float time_press;

    public void Initialize(int hit_count, FocusEvent focus_event)
    {
        FocusEvent = focus_event;
        Cursor = focus_event.Cursor;
        HitCountMax = hit_count;
        HitCount = hit_count;

        GlobalPosition = Cursor.GlobalPosition;
        UpdateSize();
        AnimationPlayer.Play("show");
        FocusCursor.MoveLock.SetLock(nameof(CursorCrystal), true);
        FocusEventView.Instance.ShowInputPrompt("Interact", GlobalPosition.Add(z: 0.7f), InputPromptFocus.AnimationType.LongPress);

        FocusEvent.OnEnded += FocusEvent_Ended;

        pressed = false;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        FocusEvent.OnEnded -= FocusEvent_Ended;
        FocusCursor.MoveLock.SetLock(nameof(CursorCrystal), false);
        FocusEventView.Instance.HideInputPrompt();
    }

    private void FocusEvent_Ended(FocusEventResult result)
    {
        QueueFree();
    }

    private void Destroy()
    {
        this.StartCoroutine(Cr, "destroy");
        IEnumerator Cr()
        {
            yield return AnimationPlayer.PlayAndWaitForAnimation("hide");
            QueueFree();
        }
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (!FocusEvent.IsRunning) return;
        if (Completed) return;

        if (PlayerInput.Interact.Pressed)
        {
            pressed = true;
            time_press = GameTime.Time + 0.3f;
            SfxCharge.Play();
            AnimationPlayer.Play("charge");
            ChargeEffect.Spawn();
        }
        else if (PlayerInput.Interact.Released)
        {
            if (!pressed) return;
            pressed = false;

            if (GameTime.Time > time_press)
            {
                DecreaseCount();
            }
            else
            {
                AnimationPlayer.Play("charge_fail");
            }
        }
    }

    private void DecreaseCount()
    {
        HitCount--;
        UpdateSize();

        if (Completed)
        {
            FocusCursor.MoveLock.SetLock(nameof(CursorCrystal), false);
            FocusEventView.Instance.HideInputPrompt();
            SfxBreak.Play();
            BreakEffect.Spawn();
            Destroy();
        }
        else
        {
            Shake();
            SfxHit.Play();
        }
    }

    private void Shake()
    {
        this.StartCoroutine(Cr, "shake");
        IEnumerator Cr()
        {
            AnimateRotate();

            HitEffect.Spawn();
            yield return AnimationPlayer.PlayAndWaitForAnimation("shake");
        }
    }

    private void AnimateRotate()
    {
        this.StartCoroutine(Cr, "rotate");
        IEnumerator Cr()
        {
            var curve = Curves.EaseOutQuad;
            var mul = rng.RandiRange(0, 1) == 0 ? 1 : -1;
            var start = SizeNode.GlobalRotationDegrees;
            var end = start + Vector3.Up * 20 * mul;
            yield return LerpEnumerator.Lerp01(0.2f, f =>
            {
                var t = curve.Evaluate(f);
                SizeNode.GlobalRotationDegrees = start.Lerp(end, t);
            });
        }
    }

    private void UpdateSize() => UpdateSize(HitCount, HitCountMax);
    private void UpdateSize(float count, float count_max)
    {
        var min = 0.7f;
        var max = 0.9f + 0.05f * count_max;
        var t = 1f - (float)count / count_max;
        var size = Mathf.Lerp(max, min, t);
        SizeNode.Scale = Vector3.One * size;
    }
}
