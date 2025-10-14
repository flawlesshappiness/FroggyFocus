using System;
using System.Collections;

public partial class FloatingPlatformController : SingletonController
{
    public static FloatingPlatformController Instance => Singleton.Get<FloatingPlatformController>();
    public override string Directory => "Objects";

    public event Action<int> OnTick;

    private int tick;
    private Coroutine cr_tick;

    public void Start()
    {
        tick = 0;

        cr_tick = this.StartCoroutine(Cr, "tick");
        IEnumerator Cr()
        {
            while (true)
            {
                OnTick?.Invoke(tick);
                yield return new WaitForSeconds(0.5f);
                tick = (tick + 1) % 100;
            }
        }
    }

    public void Stop()
    {
        Coroutine.Stop(cr_tick);
    }
}
