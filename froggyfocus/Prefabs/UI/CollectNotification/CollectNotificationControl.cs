using Godot;
using System.Collections;

public partial class CollectNotificationControl : Control
{
    [Export]
    public Label Label;

    [Export]
    public AnimationPlayer Animation;

    public Coroutine AnimateShow()
    {
        return this.StartCoroutine(Cr, "animate");
        IEnumerator Cr()
        {
            yield return Animation.PlayAndWaitForAnimation("show");
            yield return new WaitForSeconds(2f);
            yield return Animation.PlayAndWaitForAnimation("hide");
            QueueFree();
        }
    }
}
