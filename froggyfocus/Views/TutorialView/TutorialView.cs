using Godot;
using System.Collections;

public partial class TutorialView : View
{
    public static TutorialView Instance => Get<TutorialView>();

    [Export]
    public AnimationPlayer Animation_Sign;

    [Export]
    public Control JumpTutorial;

    [Export]
    public Control BugsTutorial;

    [Export]
    public Control ShieldTutorial;

    private bool input_received;

    protected override void OnShow()
    {
        base.OnShow();
        Player.SetAllLocks(nameof(TutorialView), true);
    }

    protected override void OnHide()
    {
        base.OnHide();
        Player.SetAllLocks(nameof(TutorialView), false);
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (PlayerInput.Interact.Pressed)
        {
            input_received = true;
        }
    }

    public void ShowSign(TutorialSign.Type type)
    {
        SetSignText(type);

        this.StartCoroutine(Cr, "sign");
        IEnumerator Cr()
        {
            Show();
            yield return Animation_Sign.PlayAndWaitForAnimation("show");

            input_received = false;
            while (!input_received) yield return null;

            yield return Animation_Sign.PlayAndWaitForAnimation("hide");
            Hide();
        }
    }

    private void SetSignText(TutorialSign.Type type)
    {
        JumpTutorial.Visible = type == TutorialSign.Type.Jump;
        BugsTutorial.Visible = type == TutorialSign.Type.Bugs;
        ShieldTutorial.Visible = type == TutorialSign.Type.Shield;
    }
}
