using Godot;
using System.Collections;

public partial class CrystalScene : GameScene
{
    [Export]
    public AnimatedPathFollow3D IntroCameraPath;

    [Export]
    public AudioStreamPlayer SfxIntro;

    public void StartIntro()
    {
        var id = "intro";
        this.StartCoroutine(Cr, "intro_camera");
        IEnumerator Cr()
        {
            Player.SetInputDisabled(id, true);

            SfxIntro.Play();

            IntroCameraPath.Camera.Current = true;
            yield return IntroCameraPath.Animate();

            TransitionView.Instance.StartTransition(new TransitionSettings
            {
                Type = TransitionType.Color,
                Color = Colors.Black,
                Duration = 2f,
                OnTransition = OnTransition
            });
        }

        void OnTransition()
        {
            Player.SetInputDisabled(id, false);
            Player.Instance.SetCameraTarget();
        }
    }
}
