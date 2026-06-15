using Godot;
using Godot.Collections;
using System.Collections;

public partial class EndTextView : View
{
    public static EndTextView Instance => Get<EndTextView>();

    [Export]
    public AnimationPlayer AnimationPlayer;

    [Export]
    public AudioStreamPlayer SfxText;

    [Export]
    public Array<Label> Labels;

    protected override void OnShow()
    {
        base.OnShow();
        Player.SetInputDisabled(nameof(EndTextView), true);
        AnimationPlayer.Play("show");
        InitializeLabels();
        AnimateLabels();
    }

    protected override void OnHide()
    {
        base.OnHide();
        Player.SetInputDisabled(nameof(EndTextView), false);
    }

    private void InitializeLabels()
    {
        Labels.ForEach(x => x.Hide());
    }

    private void AnimateLabels()
    {
        this.StartCoroutine(Cr, "animate");
        IEnumerator Cr()
        {
            yield return new WaitForSeconds(2f);
            foreach (var label in Labels)
            {
                label.Show();
                SfxText.Play();
                yield return new WaitForSeconds(6f);
                label.Hide();
                yield return new WaitForSeconds(1f);
            }

            yield return AnimationPlayer.PlayAndWaitForAnimation("hide");
            Hide();
        }
    }
}
