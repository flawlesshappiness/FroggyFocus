using Godot;
using System.Collections;
using System.Linq;

public partial class ThunderController : ResourceController<ThunderCollection, ThunderInfo>
{
    public static ThunderController Instance => Singleton.Get<ThunderController>();
    public override string Directory => "Thunder";

    private Coroutine cr_thunder;
    private RandomNumberGenerator rng = new();

    public void StartThunder()
    {
        StopThunder();
        cr_thunder = StartThunderLoop();
    }

    public void StopThunder()
    {
        Coroutine.Stop(cr_thunder);
    }

    private Coroutine StartThunderLoop()
    {
        return this.StartCoroutine(Cr, "thunder");
        IEnumerator Cr()
        {
            while (true)
            {
                yield return new WaitForSeconds(rng.RandfRange(10, 60));

                var thunder = CreateThunder();
                yield return thunder.Play();
                thunder.Destroy(10);
            }
        }
    }

    private ThunderCloudEffect CreateThunder()
    {
        var info = GetRandomInfo();
        var thunder = info.Scene.Instantiate<ThunderCloudEffect>();
        thunder.SetParent(Scene.Current);
        thunder.GlobalPosition = GetRandomPosition();
        thunder.SetIntensity(rng.Randf());
        return thunder;
    }

    private ThunderInfo GetRandomInfo()
    {
        return Collection.Resources.ToList().Random();
    }

    private Vector3 GetRandomPosition()
    {
        var x = rng.RandfRange(-100f, 100f);
        var z = rng.RandfRange(-100f, 100f);
        return new Vector3(x, 0, z);
    }
}
