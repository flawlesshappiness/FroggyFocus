using Godot;
using Godot.Collections;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class WeatherController : ResourceController<WeatherCollection, WeatherInfo>
{
    public static WeatherController Instance => Singleton.Get<WeatherController>();
    public override string Directory => "Weather";

    private bool skip;
    private bool quick_transition;
    private WeatherInfo current_weather;
    private WeatherInfo next_weather;
    private Coroutine cr_weather;

    private const float TRANSITION_DURATION = 30;
    private const float TRANSITION_DURATION_QUICK = 3;

    public override void _Ready()
    {
        base._Ready();
        RegisterDebugActions();
    }

    private void RegisterDebugActions()
    {
        var category = "WEATHER";

        Debug.RegisterAction(new DebugAction
        {
            Category = category,
            Text = "Skip",
            Action = Skip
        });

        Debug.RegisterAction(new DebugAction
        {
            Category = category,
            Text = "Select weather",
            Action = SelectWeather
        });

        void SelectWeather(DebugView v)
        {
            v.SetContent_Search();

            foreach (var info in Collection.Resources)
            {
                v.ContentSearch.AddItem(info.GetResourceName(), () => SetAction(v, info));
            }

            v.ContentSearch.UpdateButtons();
        }

        void SetAction(DebugView v, WeatherInfo info)
        {
            v.SetContent_Search();

            v.ContentSearch.AddItem("Fade", () => SetWeatherDebug(v, info));
            v.ContentSearch.AddItem("Fade fast", () => SetWeatherDebugFast(v, info));

            v.ContentSearch.UpdateButtons();
        }

        void SetWeatherDebug(DebugView v, WeatherInfo info)
        {
            next_weather = info;
            skip = true;
            quick_transition = false;
            v.Close();
        }

        void SetWeatherDebugFast(DebugView v, WeatherInfo info)
        {
            next_weather = info;
            skip = true;
            quick_transition = true;
            v.Close();
        }

        void Skip(DebugView v)
        {
            skip = true;
            v.Close();
        }
    }

    public void StopWeather()
    {
        Coroutine.Stop(cr_weather);
        RainController.Instance.StopRain();
        WindController.Instance.StopWind();
        ThunderController.Instance.StopThunder();
    }

    public void StartWeather(Array<WeatherInfo> weathers)
    {
        var scene = GameScene.Instance;
        var env = scene.WorldEnvironment.Environment.Duplicate() as Environment;
        scene.WorldEnvironment.Environment = env;

        var sky_material = env.Sky.SkyMaterial.Duplicate() as ShaderMaterial;
        env.Sky.SkyMaterial = sky_material;

        RainController.Instance.StartRain();
        WindController.Instance.StartWind();

        current_weather = GetNextWeather(weathers);
        SetWeatherTransition(current_weather, current_weather, 1);

        cr_weather = this.StartCoroutine(Cr, "weather");
        IEnumerator Cr()
        {
            var rng = new RandomNumberGenerator();
            while (true)
            {
                yield return WaitForSkip(rng.RandfRange(200, 400));

                var previous_weather = current_weather;
                current_weather = GetNextWeather(weathers);

                var transition_duration = quick_transition ? TRANSITION_DURATION_QUICK : TRANSITION_DURATION;
                yield return WaitForWeatherTransition(previous_weather, current_weather, transition_duration);
                quick_transition = false;
            }
        }

        IEnumerator WaitForSkip(float duration)
        {
            var start = GameTime.Time;
            var end = start + duration;
            while (GameTime.Time < end)
            {
                if (skip || next_weather != null)
                {
                    skip = false;
                    break;
                }

                yield return null;
            }
        }
    }

    private IEnumerator WaitForWeatherTransition(WeatherInfo from, WeatherInfo to, float duration)
    {
        var start = GameTime.Time;
        var end = start + duration;
        while (GameTime.Time < end)
        {
            if (skip)
            {
                skip = false;
                break;
            }

            var t = (GameTime.Time - start) / duration;
            SetWeatherTransition(from, to, t);
            yield return null;
        }

        SetWeatherTransition(from, to, 1);
    }

    private WeatherInfo GetNextWeather(IEnumerable<WeatherInfo> weathers)
    {
        var next = next_weather ?? weathers
            .Where(x => x != current_weather) // Not the same as current weather
            .Where(x => current_weather != null && current_weather.Rain > 0.0f ? x.Rain < 0.01f : true) // No repeat rain
            .ToList()
            .Random();
        next ??= weathers.ToList().Random();
        next_weather = null;
        return next;
    }

    private void SetWeatherTransition(WeatherInfo from, WeatherInfo to, float t)
    {
        var env = GameScene.Instance.WorldEnvironment.Environment;
        var sun = GameScene.Instance.DirectionalLight;

        // Sun
        sun.LightColor = from.SunColor.Lerp(to.SunColor, Mathf.Clamp(t * 10, 0, 1));
        sun.ShadowEnabled = true;
        sun.ShadowOpacity = Mathf.Lerp(from.SunShadowOpacity, to.SunShadowOpacity, t);

        // Fog
        env.FogEnabled = true;
        env.FogAerialPerspective = Mathf.Lerp(from.FogEnabled ? 0 : 1, to.FogEnabled ? 0 : 1, t);
        env.FogDensity = Mathf.Lerp(from.FogEnabled ? from.FogDensity : 0, to.FogEnabled ? to.FogDensity : 0, t);
        env.FogLightColor = from.FogColor.Lerp(to.FogEnabled ? to.FogColor : from.FogColor, t);

        // Rain
        RainController.Instance.SetIntensity(Mathf.Lerp(from.Rain, to.Rain, t));

        // Thunder
        if (!to.Thunder)
        {
            ThunderController.Instance.StopThunder();
        }
        else if (t >= 1.0f)
        {
            ThunderController.Instance.StartThunder();
        }

        // Wind
        var wind_from = from.Wind ? from.WindRange : Vector2.Zero;
        var wind_to = to.Wind ? to.WindRange : Vector2.Zero;
        var wind = wind_from.Lerp(wind_to, t);
        WindController.Instance.SetIntensityRange(wind);
    }
}
