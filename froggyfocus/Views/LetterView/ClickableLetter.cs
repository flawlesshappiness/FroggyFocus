using Godot;
using Godot.Collections;
using System.Collections;

public partial class ClickableLetter : VBoxContainer
{
    [Export]
    public ClickableLetterArrow Arrow;

    [Export]
    public AudioStreamPlayer SfxText;

    [Export]
    public Array<Label> Labels;

    private int idx_label = 0;
    private bool waiting_for_input;

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (waiting_for_input && PlayerInput.Interact.Pressed)
        {
            waiting_for_input = false;
        }
    }

    public void HideLabels()
    {
        idx_label = 0;
        foreach (var label in Labels)
        {
            label.VisibleCharacters = 0;
            label.Hide();
        }

        Arrow.Hide();
    }

    public Coroutine AnimateLabels()
    {
        return this.StartCoroutine(Cr, "labels");
        IEnumerator Cr()
        {
            while (idx_label < Labels.Count)
            {
                Arrow.Hide();
                yield return AnimateLabel();
                idx_label++;

                Arrow.Show();
                waiting_for_input = true;
                while (waiting_for_input)
                    yield return null;
            }
        }
    }

    public Coroutine AnimateLabel()
    {
        var label = Labels[idx_label];
        label.Show();

        SfxText.Play();

        return this.StartCoroutine(Cr, "show_label");
        IEnumerator Cr()
        {
            var time_next = GameTime.Time;
            var idx_char = 0;
            label.VisibleCharacters = idx_char;

            while (idx_char < label.Text.Length)
            {
                while (time_next < GameTime.Time)
                {
                    idx_char++;
                    label.VisibleCharacters = idx_char;
                    time_next += 0.02f;
                }

                yield return null;
            }
        }
    }
}
