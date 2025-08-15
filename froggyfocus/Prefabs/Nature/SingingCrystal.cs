using Godot;
using System.Collections;

public partial class SingingCrystal : Node3D
{
    [Export]
    public AudioStreamPlayer3D SfxCrystal;

    public override void _Ready()
    {
        base._Ready();
        StartSinging();
    }

    private Coroutine StartSinging()
    {
        var rng = new RandomNumberGenerator();
        return this.StartCoroutine(Cr, "sing");
        IEnumerator Cr()
        {
            while (true)
            {
                var delay = rng.RandfRange(5f, 20f);
                yield return new WaitForSeconds(delay);

                SfxCrystal.Play();

                var length = SfxCrystal.Stream.GetLength();
                yield return new WaitForSeconds(length);
            }
        }
    }
}
