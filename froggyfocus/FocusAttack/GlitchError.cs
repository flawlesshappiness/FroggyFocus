using FlawLizArt.FocusEvent;
using Godot;
using System.Collections;
using System.Collections.Generic;

public partial class GlitchError : FocusAttack
{
    [Export]
    public AudioStreamPlayer SfxError;

    private Coroutine cr_run;
    private Coroutine cr_wait;

    protected override void CursorEnter()
    {
        base.CursorEnter();
        var duration = rng.RandfRange(2, 6);
        cr_wait = WaitFor(duration, Spawn);
    }

    protected override void CursorExit()
    {
        base.CursorExit();
        Coroutine.Stop(cr_wait);
    }

    protected override void Stopped()
    {
        base.Stopped();
        Coroutine.Stop(cr_run);
    }

    private void Spawn()
    {
        this.StartCoroutine(Cr, "spawn");
        IEnumerator Cr()
        {
            StartState();

            var count = rng.RandiRange(6, 9);
            var positions = GetPositions(count);
            var delay_range = new Vector2(0.4f, 0.05f);
            var pitch_start = rng.RandfRange(0f, 0.25f);
            var pitch_range = new Vector2(0.5f, 1.5f);
            var curve = Curves.EaseOutQuad;

            SfxError.PitchScale = pitch_range.X;
            SfxError.Play();

            for (int i = 0; i < positions.Count; i++)
            {
                var position = positions[i];
                var t = (float)i / (positions.Count - 1);
                var tc = curve.Evaluate(t);
                var delay = delay_range.Range(tc);
                var pitch = pitch_start + pitch_range.Range(tc);

                SfxError.PitchScale = pitch;
                Target.GlobalPosition = position;
                yield return new WaitForSeconds(delay);
            }

            SfxError.Stop();

            if (IsFocusTarget)
            {
                HurtFocusValue(0.1f);
                DisruptCursorFocus();
                AnimateMoveCursorAway();
            }

            EndState();
        }
    }

    private List<Vector3> GetPositions(int count)
    {
        var list = new List<Vector3>();
        for (int i = 0; i < count; i++)
        {
            list.Add(Target.GetRandomPosition());
        }
        return list;
    }
}
