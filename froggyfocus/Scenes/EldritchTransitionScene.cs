using Godot;
using System.Collections;

public partial class EldritchTransitionScene : Scene
{
    [Export]
    public AnimationPlayer AnimationPlayer;

    public IEnumerator StartTransition_Enter()
    {
        yield return AnimationPlayer.PlayAndWaitForAnimation("enter");
    }

    public IEnumerator StartTransition_Exit()
    {
        yield return AnimationPlayer.PlayAndWaitForAnimation("exit");
    }
}
