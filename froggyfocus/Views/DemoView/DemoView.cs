using Godot;
using System.Collections;

public partial class DemoView : View
{
    public static DemoView Instance => Get<DemoView>();

    [Export]
    public Button ContinueButton;

    [Export]
    public Control ModulateControl;

    [Export]
    public Control InputBlocker;

    public override void _Ready()
    {
        base._Ready();

        ModulateControl.Modulate = Colors.White.SetA(0);
        ContinueButton.Pressed += ContinueButton_Pressed;
    }

    protected override void OnShow()
    {
        base.OnShow();
        Player.SetAllLocks(nameof(DemoView), true);
        MouseVisibility.Show(nameof(DemoView));
    }

    protected override void OnHide()
    {
        base.OnHide();
        Player.SetAllLocks(nameof(DemoView), false);
        MouseVisibility.Hide(nameof(DemoView));
    }

    private void ContinueButton_Pressed()
    {
        AnimateHide();
    }

    public void AnimateShow()
    {
        Show();
        ReleaseCurrentFocus();
        InputBlocker.Show();

        this.StartCoroutine(Cr, "animate");
        IEnumerator Cr()
        {
            var start = Colors.White.SetA(0);
            var end = Colors.White;
            yield return LerpEnumerator.Lerp01(1f, f =>
            {
                ModulateControl.Modulate = start.Lerp(end, f);
            });

            InputBlocker.Hide();
            ContinueButton.GrabFocus();
        }
    }

    private void AnimateHide()
    {
        ReleaseCurrentFocus();
        InputBlocker.Show();

        this.StartCoroutine(Cr, "animate");
        IEnumerator Cr()
        {
            var start = Colors.White;
            var end = Colors.White.SetA(0);
            yield return LerpEnumerator.Lerp01(1f, f =>
            {
                ModulateControl.Modulate = start.Lerp(end, f);
            });

            InputBlocker.Hide();
            Hide();
        }
    }
}
