using Godot;
using Godot.Collections;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class LetterScene : GameScene
{
    [Export]
    public AnimationPlayer AnimationPlayer;

    [Export]
    public Array<Letter> Letters;

    private int idx_letter;
    private bool letter_input_enabled;
    private Letter selected_letter;
    private List<Letter> unpicked_letters;

    public override void _Ready()
    {
        base._Ready();
        MouseVisibility.Show(nameof(LetterScene));

        InitializeLetters();
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        MouseVisibility.Hide(nameof(LetterScene));
    }

    private void InitializeLetters()
    {
        unpicked_letters = Letters.ToList();

        foreach (var letter in Letters)
        {
            letter.OnMouseEnter += () => Letter_MouseEnter(letter);
        }

        this.StartCoroutine(Cr, "letters");
        IEnumerator Cr()
        {
            letter_input_enabled = false;
            yield return AnimationPlayer.PlayAndWaitForAnimation("show_letters");
            letter_input_enabled = true;
            SetLetterIndex(0);
        }
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (PlayerInput.Right.Pressed)
        {
            SetLetterIndex(idx_letter + 1);
        }
        else if (PlayerInput.Left.Pressed)
        {
            SetLetterIndex(idx_letter - 1);
        }
        else if (PlayerInput.Interact.Pressed)
        {
            PickSelectedLetter();
        }
    }

    private void Letter_MouseEnter(Letter letter)
    {
        SelectLetter(letter);
    }

    private void SetLetterIndex(int i)
    {
        idx_letter = Mathf.Clamp(i, 0, unpicked_letters.Count);
        var letter = unpicked_letters.GetClamped(i);
        SelectLetter(letter);
    }

    private void SelectLetter(Letter letter)
    {
        if (!letter_input_enabled) return;
        if (selected_letter == letter) return;
        if (!unpicked_letters.Contains(letter)) return;

        selected_letter?.AnimateLower();
        selected_letter = letter;
        selected_letter?.AnimateRaise();

        idx_letter = unpicked_letters.IndexOf(selected_letter);
    }

    private void PickSelectedLetter()
    {
        if (!letter_input_enabled) return;

        this.StartCoroutine(Cr, "letter");
        IEnumerator Cr()
        {
            letter_input_enabled = false;
            yield return selected_letter.AnimateOpen();

            LetterView.Instance.Show();
            yield return LetterView.Instance.AnimateLetter(selected_letter.Id);
            LetterView.Instance.Hide();

            unpicked_letters.Remove(selected_letter);

            if (unpicked_letters.Count > 0)
            {
                letter_input_enabled = true;
                selected_letter = null;
                SetLetterIndex(idx_letter);
            }
            else
            {
                TransitionView.Instance.StartTransition(new TransitionSettings
                {
                    Type = TransitionType.Color,
                    Color = Colors.Black,
                    Duration = 2f,
                    OnTransition = () =>
                    {
                        Data.Game.CurrentScene = nameof(SwampScene);
                        Data.Game.StartingNode = "";
                        Data.Game.Save();

                        Scene.Goto<SwampScene>();
                    }
                });
            }
        }
    }
}
