using Godot;
using System.Collections;

public partial class QuestionMarkEffect : Node3D
{
    [Export]
    public AnimationPlayer AnimationPlayer;

    private bool visible;

    public IEnumerator AnimateShow()
    {
        if (visible)
        {
            return null;
        }
        else
        {
            visible = true;
            return AnimationPlayer.PlayAndWaitForAnimation("show");
        }
    }

    public IEnumerator AnimateHide()
    {
        if (visible)
        {
            visible = false;
            return AnimationPlayer.PlayAndWaitForAnimation("hide");
        }
        else
        {
            return null;
        }
    }
}
