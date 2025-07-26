
using Godot;
using System.Collections;
using System.Linq;

public partial class FocusHotSpotController : SingletonController
{
    public static FocusHotSpotController Instance => Singleton.Get<FocusHotSpotController>();
    public override string Directory => "FocusHotSpot";

    private const string HotSpotTemplatePath = $"res://Prefabs/FocusEvent/focus_hot_spot.tscn";

    private bool skip;
    private Coroutine cr_hotspots;
    private RandomNumberGenerator rng = new();

    public override void _Ready()
    {
        base._Ready();
        RegisterDebugActions();
    }

    private void RegisterDebugActions()
    {
        var category = "HOT SPOTS";

        Debug.RegisterAction(new DebugAction
        {
            Category = category,
            Text = "Skip",
            Action = Skip
        });

        void Skip(DebugView v)
        {
            skip = true;
            v.Close();
        }
    }

    public void Stop()
    {
        Coroutine.Stop(cr_hotspots);
    }

    public void Start()
    {
        Stop();

        cr_hotspots = this.StartCoroutine(Cr, "hotspots");
        IEnumerator Cr()
        {
            while (true)
            {
                var duration = rng.RandfRange(40f, 60f);
                var end = GameTime.Time + duration;
                while (GameTime.Time < end)
                {
                    if (skip)
                    {
                        skip = false;
                        break;
                    }
                    yield return null;
                }

                var hotspot = CreateHotSpot();
                hotspot.DestroyAfterDelay(40f);
            }
        }
    }

    private FocusHotSpot CreateHotSpot()
    {
        var hotspot = GDHelper.Instantiate<FocusHotSpot>(HotSpotTemplatePath);
        hotspot.SetParent(Scene.Current);
        var d = 20f;
        var x = rng.RandfRange(-d, d);
        var z = rng.RandfRange(-d, d);
        var position = Player.Instance.GlobalPosition + new Vector3(x, 0, z);
        var nav_position = NavigationServer3D.MapGetClosestPoint(NavigationServer3D.GetMaps().First(), position);
        hotspot.GlobalPosition = nav_position;
        return hotspot;
    }
}
