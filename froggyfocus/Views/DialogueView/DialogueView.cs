using Godot;
using System;
using System.Collections;

public partial class DialogueView : View
{
    [Export]
    public AnimatedPanel AnimatedPanel_Dialogue;

    [Export]
    public Label DialogueLabel;

    [Export]
    public AudioStreamPlayer SfxRevealCharacter;

    public event Action OnNextDialogue;

    private bool has_active_dialogue;
    private bool input_received;
    private bool is_revealing_text;
    private float time_reveal_character_sfx;

    public override void _Ready()
    {
        base._Ready();
        RegisterDebugActions();
    }

    private void RegisterDebugActions()
    {
        var category = "DIALOGUE";

        Debug.RegisterAction(new DebugAction
        {
            Category = category,
            Text = "Test view",
            Action = DebugTest
        });

        void DebugTest(DebugView v)
        {
            v.Close();
            AnimateDialogue("Bla bla bla bla bla bla bla bla bla bla bla bla bla bla bla bla bla bla bla bla bla bla bla bla bla bla bla bla bla bla bla bla bla bla bla bla bla bla bla bla");
        }
    }

    protected override void OnShow()
    {
        base.OnShow();
        Player.SetAllLocks(nameof(DialogueView), true);
    }

    protected override void OnHide()
    {
        base.OnHide();
        Player.SetAllLocks(nameof(DialogueView), false);
    }

    public override void _Input(InputEvent e)
    {
        base._Input(e);

        var is_mouse = e is InputEventMouseButton me && me.ButtonIndex == MouseButton.Left && me.Pressed;
        var is_interact = PlayerInput.Interact.Pressed;
        if (is_mouse || is_interact)
        {
            Skip();
        }
    }

    public void AnimateDialogue(string text)
    {
        DialogueLabel.Text = text;
        DialogueLabel.VisibleCharacters = 0;
        has_active_dialogue = true;

        this.StartCoroutine(Cr, "animate");
        IEnumerator Cr()
        {
            if (!Visible)
            {
                Show();
                yield return AnimatedPanel_Dialogue.AnimatePopShow();
            }
            else
            {
                AnimatedPanel_Dialogue.AnimateBounce();
            }

            input_received = false;
            yield return RevealTextSequence();
        }
    }

    public void AnimateClose()
    {
        has_active_dialogue = false;

        this.StartCoroutine(Cr, "animate");
        IEnumerator Cr()
        {
            if (Visible)
            {
                yield return AnimatedPanel_Dialogue.AnimatePopHide();
                Hide();
            }
        }
    }

    private IEnumerator RevealTextSequence()
    {
        is_revealing_text = true;

        var character_time = 0.005f;
        while (DialogueLabel.VisibleCharacters < DialogueLabel.Text.Length)
        {
            DialogueLabel.VisibleCharacters++;
            PlayRevealCharacterSfx();

            var time_next = GameTime.Time + character_time;
            while (GameTime.Time < time_next)
            {
                if (input_received)
                {
                    DialogueLabel.VisibleCharacters = DialogueLabel.Text.Length;
                }

                yield return null;
            }
        }

        is_revealing_text = false;
    }

    private void PlayRevealCharacterSfx()
    {
        if (GameTime.Time < time_reveal_character_sfx) return;
        time_reveal_character_sfx = GameTime.Time + 0.08f;

        SfxRevealCharacter.Play();
    }

    private void Skip()
    {
        if (is_revealing_text)
        {
            input_received = true;
        }
        else if (has_active_dialogue)
        {
            Next();
        }
    }

    private void Next()
    {
        OnNextDialogue?.Invoke();
    }
}