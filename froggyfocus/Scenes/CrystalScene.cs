using Godot;
using System.Collections;

public partial class CrystalScene : GameScene
{
    [Export]
    public AnimationPlayer AnimationPlayer_Intro;

    public void StartIntro()
    {
        this.StartCoroutine(Cr, "intro");
        IEnumerator Cr()
        {
            var id = nameof(CrystalScene);
            Player.SetAllLocks(id, true);
            yield return AnimationPlayer_Intro.PlayAndWaitForAnimation("intro");
            TransitionView.Instance.StartTransition(new TransitionSettings
            {
                Type = TransitionType.Color,
                Color = Colors.Black,
                Duration = 1f,
                OnTransition = () =>
                {
                    Player.Instance.SetCameraTarget();
                }
            });
            Player.SetAllLocks(id, false);

            MainQuestController.Instance.AdvancePartnerQuest(3);
        }
    }
}
