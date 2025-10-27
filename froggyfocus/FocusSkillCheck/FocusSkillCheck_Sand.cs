using Godot;
using Godot.Collections;
using System.Collections;

public partial class FocusSkillCheck_Sand : FocusSkillCheck
{
    [Export]
    public Vector2 AmountRange;

    [Export]
    public Vector2 DurationRange;

    [Export]
    public GpuParticles3D PsWind;

    [Export]
    public AudioStreamPlayer SfxWind;

    [Export]
    public Array<Godot.Curve> Curves;

    public override void _Ready()
    {
        base._Ready();
        SfxWind.Play();
        SfxWind.VolumeLinear = 0;
    }

    public override void Clear()
    {
        base.Clear();
        PsWind.Emitting = false;
        SfxWind.VolumeLinear = 0;
    }

    protected override IEnumerator Run()
    {
        StartWind();
        yield return null;
    }

    private Coroutine StartWind()
    {
        var angle = rng.RandfRange(0f, 360f);
        var dir = Vector3.Forward.Rotated(Vector3.Up, Mathf.DegToRad(angle));
        PsWind.GlobalRotationDegrees = new Vector3(0f, angle, 0f);

        var wind_amount = AmountRange.Range(Difficulty);
        var wind_duration = DurationRange.Range(Difficulty);
        var time_start = GameTime.Time;
        var time_end = time_start + wind_duration;
        var curve = Curves.PickRandom();

        return this.StartCoroutine(Cr, "wind");
        IEnumerator Cr()
        {
            PsWind.Emitting = true;

            SfxWind.FadeIn(0.5f, 0);

            while (GameTime.Time < time_end)
            {
                var t = (GameTime.Time - time_start) / wind_duration;
                var amount = wind_amount * curve.Sample(Mathf.Clamp(t, 0, 1));
                FocusEvent.Cursor.GlobalPosition += dir * amount;
                yield return null;
            }

            yield return SfxWind.FadeOut(0.5f);

            Clear();
        }
    }
}
