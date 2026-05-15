using Godot;
using System.Collections;

namespace FlawLizArt.FocusEvent;

public partial class CursorSyringeLiquid : TrapObject
{
    [Export]
    public AnimationPlayer AnimationPlayer;

    [Export]
    public EffectGroupSpawner ShakeEffect;

    protected override void Started()
    {
        base.Started();
        ShakeEffect.Spawn();
        AnimationPlayer.Replay("show");
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
        ShakeEffect.Spawn();

        if (Completed)
        {
            AnimateComplete();
        }
        else
        {
            AnimationPlayer.Replay("shake");
        }
    }

    private void AnimateComplete()
    {
        this.StartCoroutine(Cr);
        IEnumerator Cr()
        {
            yield return AnimationPlayer.PlayAndWaitForAnimation("hide");
            QueueFree();
        }
    }
}
