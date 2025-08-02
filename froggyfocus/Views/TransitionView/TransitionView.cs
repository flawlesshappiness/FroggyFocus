using Godot;
using System;
using System.Collections;

public partial class TransitionView : View
{
    public static TransitionView Instance => Get<TransitionView>();

    [Export]
    public AnimationPlayer AnimationPlayer;

    public void StartTransition(Action on_full)
    {
        this.StartCoroutine(Cr, "transition");
        IEnumerator Cr()
        {
            Show();
            yield return AnimationPlayer.PlayAndWaitForAnimation("shape_in");
            on_full?.Invoke();
            yield return AnimationPlayer.PlayAndWaitForAnimation("shape_out");
            Hide();
        }
    }
}
