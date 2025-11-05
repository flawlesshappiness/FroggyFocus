using Godot;
using System.Collections;

[GlobalClass]
public partial class ReverbArea : Area3D
{
    [Export(PropertyHint.Range, "0,1,0.001")]
    public float Amount;

    private AudioBus bus;
    private AudioEffectReverb reverb;

    public override void _Ready()
    {
        base._Ready();
        BodyEntered += PlayerEntered;
        BodyExited += PlayerExited;

        bus = AudioBus.Get(AudioBusNames.SFX);
        reverb = bus.GetEffect<AudioEffectReverb>();
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        reverb.Wet = 0;
    }

    private void PlayerEntered(GodotObject go)
    {
        FadeReverb(Amount, 1f);
    }

    private void PlayerExited(GodotObject go)
    {
        FadeReverb(0, 1f);
    }

    private Coroutine FadeReverb(float amount, float duration)
    {
        return this.StartCoroutine(Cr, "fade");
        IEnumerator Cr()
        {
            var start = reverb.Wet;
            var end = amount;
            yield return LerpEnumerator.Lerp01(duration, f =>
            {
                reverb.Wet = Mathf.Lerp(start, end, f);
            });
        }
    }
}
