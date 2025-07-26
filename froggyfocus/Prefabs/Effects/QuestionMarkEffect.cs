using Godot;
using System.Collections;

public partial class QuestionMarkEffect : Node3D
{
    [Export]
    public AnimationPlayer AnimationPlayer;

    public IEnumerator AnimateShow()
    {
        return AnimationPlayer.PlayAndWaitForAnimation("show");
    }

    public IEnumerator AnimateHide()
    {
        return AnimationPlayer.PlayAndWaitForAnimation("hide");
    }
}
