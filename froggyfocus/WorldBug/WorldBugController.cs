using Godot;
using System.Collections;

public partial class WorldBugController : SingletonController
{
    public static WorldBugController Instance => Singleton.Get<WorldBugController>();
    public override string Directory => "WorldBug";

    private Coroutine cr_world_bugs;
    private RandomNumberGenerator rng = new();

    public void Start()
    {
        Stop();
        cr_world_bugs = this.StartCoroutine(Cr, "world_bugs");
        IEnumerator Cr()
        {
            while (true)
            {
                yield return new WaitForSeconds(rng.RandfRange(5f, 10f));

                var world_bug = GameScene.Instance.GetClosestWorldBug();

                if (IsInstanceValid(world_bug))
                {
                    world_bug.Animate();
                }
            }
        }
    }

    public void Stop()
    {
        Coroutine.Stop(cr_world_bugs);
    }
}
