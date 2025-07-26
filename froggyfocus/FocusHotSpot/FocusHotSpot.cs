using Godot;
using System.Collections;

public partial class FocusHotSpot : Area3D
{
    [Export]
    public ParticleEffectGroup Particles;

    public string Id => $"{nameof(FocusHotSpot)}_{GetInstanceId()}";

    private bool enabled;
    private bool active;

    public override void _Ready()
    {
        base._Ready();
        BodyEntered += PlayerEntered;
        BodyExited += PlayerExited;

        FocusEventController.Instance.OnFocusEventCompleted += FocusEventCompleted;
        FocusEventController.Instance.OnFocusEventFailed += FocusEventFailed;

        enabled = true;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        FocusEventController.Instance.OnFocusEventCompleted -= FocusEventCompleted;
        FocusEventController.Instance.OnFocusEventFailed -= FocusEventFailed;
        SetLock(false);
    }

    private void PlayerEntered(GodotObject body)
    {
        SetLock(true && enabled);
    }

    private void PlayerExited(GodotObject body)
    {
        SetLock(false);
    }

    private void FocusEventCompleted(FocusEventCompletedResult result)
    {
        if (active)
        {
            Destroy();
        }
    }

    private void FocusEventFailed(FocusEventFailedResult result)
    {
        if (active)
        {
            Destroy();
        }
    }

    private void SetLock(bool locked)
    {
        active = locked;
        Player.FocusHotSpotLock.SetLock(Id, locked);
    }

    public void DestroyAfterDelay(float delay)
    {
        this.StartCoroutine(Cr, "destroy");
        IEnumerator Cr()
        {
            var end = GameTime.Time + delay;
            while (GameTime.Time < end || active)
            {
                yield return null;
            }
            Destroy();
        }
    }

    private void Destroy()
    {
        this.StartCoroutine(Cr, "destroy");
        IEnumerator Cr()
        {
            enabled = false;
            SetLock(false);
            Particles.Stop();

            yield return new WaitForSeconds(4f);

            QueueFree();
        }
    }
}
