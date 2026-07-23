using Godot;
using System.Collections;

public partial class SwampScene : GameScene
{
    [Export]
    public AnimatedPathFollow3D IntroCameraPath;

    private string DebugId => $"{nameof(SwampScene)}{GetInstanceId()}";

    public override void _Ready()
    {
        base._Ready();
        RegisterDebugActions();
    }

    protected override void Initialize()
    {
        base.Initialize();

        if (GameFlags.IsFlag(LetterScene.INTRO_LETTERS_ID, 0))
        {
            GameFlags.SetFlag(LetterScene.INTRO_LETTERS_ID, 1);
            GameView.Instance.TriggerQuestAdvancedNotification();
        }
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        Debug.RemoveActions(DebugId);
    }

    private void RegisterDebugActions()
    {
        var category = "SWAMP SCENE";
    }

    public Coroutine AnimateIntroCamera()
    {
        var id = "intro";
        return this.StartCoroutine(Cr, "intro_camera");
        IEnumerator Cr()
        {
            Player.SetInputDisabled(id, true);

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
