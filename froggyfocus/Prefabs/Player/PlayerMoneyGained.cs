using Godot;
using System.Collections;

public partial class PlayerMoneyGained : Node3D
{
    [Export]
    public AnimationPlayer AnimationPlayer;

    [Export]
    public Label3D AmountLabel;

    public Coroutine AnimateMoneyGained(int amount)
    {
        AmountLabel.Text = $"+{amount}";

        return this.StartCoroutine(Cr, "animate");
        IEnumerator Cr()
        {
            yield return AnimationPlayer.PlayAndWaitForAnimation("show");
            yield return new WaitForSeconds(1.5f);
            yield return AnimationPlayer.PlayAndWaitForAnimation("hide");
        }
    }
}
