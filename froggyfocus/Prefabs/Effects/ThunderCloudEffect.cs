using Godot;
using System.Collections;
using System.Collections.Generic;

public partial class ThunderCloudEffect : Node3D
{
    [Export]
    public AnimationPlayer AnimationPlayer_Light;

    [Export]
    public GpuParticles3D PsCloud;

    [Export]
    public AudioStreamPlayer3D SfxThunder;

    private RandomNumberGenerator rng = new();

    private List<string> lightning_anims = new()
    {
        "lightning_001",
        "lightning_002",
        "lightning_003",
        "lightning_004",
    };

    public void SetIntensity(float t)
    {
        PsCloud.AmountRatio = Mathf.Lerp(0.3f, 1.0f, t);
    }

    public Coroutine Play()
    {
        return this.StartCoroutine(Cr);
        IEnumerator Cr()
        {
            PsCloud.Emitting = true;
            yield return new WaitForSeconds(0.5f);
            AnimateLight();
            yield return new WaitForSeconds(rng.RandfRange(3f, 6f));
            SfxThunder.Play();
            yield return new WaitForSeconds(20.0f);
        }
    }

    private void AnimateLight()
    {
        var anim = lightning_anims.Random();
        AnimationPlayer_Light.Play(anim);
    }
}
