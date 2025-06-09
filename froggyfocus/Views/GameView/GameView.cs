using Godot;
using System.Collections;

public partial class GameView : View
{
    public static GameView Instance => Get<GameView>();

    [Export]
    public AnimationPlayer AnimationPlayer;

    [Export]
    public ColorRect Overlay;

    public void AnimateHideOverlay()
    {
        Overlay.Show();
        Overlay.Modulate = Overlay.Modulate.SetA(1);

        this.StartCoroutine(Cr, "transition");
        IEnumerator Cr()
        {
            yield return AnimationPlayer.PlayAndWaitForAnimation("hide_overlay");
        }
    }
}
