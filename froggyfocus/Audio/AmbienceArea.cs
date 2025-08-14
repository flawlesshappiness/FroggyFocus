using Godot;
using System.Collections;

public partial class AmbienceArea : Area3D
{
    [Export]
    public float FadeDuration;

    [Export]
    public AudioStreamPlayer Ambience;

    private float target_volume;

    public override void _Ready()
    {
        base._Ready();

        BodyEntered += PlayerEntered;
        BodyExited += PlayerExited;

        Ambience.Play();
        target_volume = Ambience.VolumeLinear;
        Ambience.VolumeLinear = 0;
    }

    private void PlayerEntered(GodotObject go)
    {
        FadeVolume(target_volume);
    }

    private void PlayerExited(GodotObject go)
    {
        FadeVolume(0);
    }

    private Coroutine FadeVolume(float volume)
    {
        return this.StartCoroutine(Cr, "volume");
        IEnumerator Cr()
        {
            var start = Ambience.VolumeLinear;
            var end = volume;
            yield return LerpEnumerator.Lerp01(FadeDuration, f =>
            {
                Ambience.VolumeLinear = Mathf.Lerp(start, end, f);
            });
        }
    }
}
