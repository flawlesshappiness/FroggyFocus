using Godot;
using System.Collections;

public partial class LetterView : View
{
    public static LetterView Instance => Get<LetterView>();

    [Export]
    public AnimationPlayer AnimationPlayer_Letter;

    [Export]
    public ClickableLetter LetterManager;

    [Export]
    public ClickableLetter LetterPartner;

    [Export]
    public ClickableLetter LetterScientist;

    private void HideLetters()
    {
        LetterManager.Hide();
        LetterPartner.Hide();
        LetterScientist.Hide();
    }

    public Coroutine AnimateLetter(string id)
    {
        HideLetters();

        var letter = GetLetter(id);
        letter.HideLabels();
        letter.Show();

        return this.StartCoroutine(Cr, "letter");
        IEnumerator Cr()
        {
            yield return AnimationPlayer_Letter.PlayAndWaitForAnimation("show");
            yield return letter.AnimateLabels();
            yield return AnimationPlayer_Letter.PlayAndWaitForAnimation("hide");
        }
    }

    private ClickableLetter GetLetter(string id) => id switch
    {
        "manager" => LetterManager,
        "partner" => LetterPartner,
        "scientist" => LetterScientist,
        _ => LetterPartner
    };
}
