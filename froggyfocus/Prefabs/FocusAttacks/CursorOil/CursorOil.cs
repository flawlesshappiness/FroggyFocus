using Godot;
using System.Collections;

namespace FlawLizArt.FocusEvent;

public partial class CursorOil : TrapObject
{
    [Export]
    public Color FlashColor;

    [Export]
    public AnimationPlayer AnimationPlayer;

    [Export]
    public EffectGroupSpawner BubblesEffect;

    [Export]
    public EffectGroupSpawner ExplosionEffect;

    protected override void Started()
    {
        base.Started();
        AnimationPlayer.Play("show");
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (!FocusEvent.IsRunning) return;
        if (Completed) return;

        if (PlayerInput.Interact.Pressed)
        {
            DecreaseCount();
        }
    }

    protected override void DecreaseCount()
    {
        base.DecreaseCount();
        UpdateSize();

        if (Completed)
        {
            AnimatePop();
        }
        else
        {
            BubblesEffect.Spawn();
            AnimationPlayer.Replay("shake");
        }
    }

    private void AnimatePop()
    {
        this.StartCoroutine(Cr, "animate");
        IEnumerator Cr()
        {
            yield return AnimationPlayer.PlayAndWaitForAnimation("hide");
            ExplosionEffect.Spawn();

            if (!FocusEvent.IsCoveringEyes)
            {
                FocusEventView.Instance.Flash(4f, FlashColor);
                FocusEventView.Instance.OilSplat(5f);
            }

            yield return new WaitForSeconds(1f);

            QueueFree();
        }
    }
}
