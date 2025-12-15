using Godot;
using Godot.Collections;
using System.Collections;

public partial class ClickableLetter : VBoxContainer
{
    [Export]
    public Array<Label> Labels;

    private int idx_label = 0;

    public void HideLabels()
    {
        idx_label = 0;
        foreach (var label in Labels)
        {
            label.VisibleCharacters = 0;
        }
    }

    public Coroutine AnimateLabels()
    {
        return this.StartCoroutine(Cr, "labels");
        IEnumerator Cr()
        {
            while (idx_label < Labels.Count)
            {
                yield return AnimateLabel();
                idx_label++;
                yield return null;
                yield return new WaitForSeconds(0.5f);
                // wait for input
            }
        }
    }

    public Coroutine AnimateLabel()
    {
        var label = Labels[idx_label];

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
