using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class WeatherController : ResourceController<WeatherCollection, WeatherInfo>
{
    public static WeatherController Instance => Singleton.Get<WeatherController>();
    public override string Directory => "Weather";

    public event Action<WeatherInfo> OnWeatherStart;

    public static MultiLock FogLock = new MultiLock();

    private bool skip;
    private Coroutine cr_weather;
    private Settings current_settings;
    private GameScene current_scene;
    private WeatherInfo current_weather;
    private RandomNumberGenerator rng = new();

    public class Settings
    {
        public List<WeatherInfo> Weathers { get; set; }
        public float? InitialTransitionDuration { get; set; } // 3
        public float TransitionDuration { get; set; } // 30
        public float WeatherDuration { get; set; }
    }

    public override void _Ready()
    {
        base._Ready();
        RegisterDebugActions();

        FogLock.OnLocked += FogLock_Changed;
        FogLock.OnFree += FogLock_Changed;
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

        Debug.RegisterAction(new DebugAction
        {
            Category = category,
            Text = "Transition to scene weather",
            Action = SceneWeather
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

            v.ContentSearch.AddItem("Normal transition", () => SetWeatherDebug(v, info));
            v.ContentSearch.AddItem("Fast transition", () => SetWeatherDebugFast(v, info));

            v.ContentSearch.UpdateButtons();
        }

        void SetWeatherDebug(DebugView v, WeatherInfo info)
        {
            StartWeather(new Settings
            {
                Weathers = new List<WeatherInfo> { info },
                TransitionDuration = 30f,
                WeatherDuration = 300f
            });
            v.Close();
        }

        void SetWeatherDebugFast(DebugView v, WeatherInfo info)
        {
            StartWeather(new Settings
            {
                Weathers = new List<WeatherInfo> { info },
                InitialTransitionDuration = 3f,
                TransitionDuration = 30f,
                WeatherDuration = 300f
            });
            v.Close();
        }

        void Skip(DebugView v)
        {
            skip = true;
            v.Close();
        }

        void SceneWeather(DebugView v)
        {
            GameScene.Instance.StartWeather();
            v.Close();
        }
    }

    private void InitializeSceneEnvironment()
    {
        if (GameScene.Instance == null) return;
        if (GameScene.Instance == current_scene) return;
        current_scene = GameScene.Instance;

        var env = current_scene.WorldEnvironment.Environment.Duplicate() as Godot.Environment;
        current_scene.WorldEnvironment.Environment = env;

        var sky_material = env.Sky.SkyMaterial.Duplicate() as ShaderMaterial;
        env.Sky.SkyMaterial = sky_material;

        RainController.Instance.StartRain();
        WindController.Instance.StartWind();
    }

    public void StopWeather()
    {
        Coroutine.Stop(cr_weather);
        RainController.Instance.StopRain();
        WindController.Instance.StopWind();
        ThunderController.Instance.StopThunder();
    }

    public void StartWeather(Settings settings)
    {
        InitializeSceneEnvironment();

        current_settings = settings;

        cr_weather = this.StartCoroutine(Cr, "weather");
        IEnumerator Cr()
        {
            // First weather
            if (current_weather == null || current_settings.InitialTransitionDuration == null)
            {
                current_weather = GetNextWeather();
                LerpWeather(current_weather, current_weather, 1);
            }
            else
            {
                var duration = current_settings.InitialTransitionDuration ?? current_settings.TransitionDuration;
                var previous_weather = current_weather;
                current_weather = GetNextWeather();
                yield return WaitForWeatherTransition(previous_weather, current_weather, duration);
            }

            // Weather loop
            while (true)
            {
                yield return WaitForSkip(current_settings.WeatherDuration);

                var previous_weather = current_weather;
                current_weather = GetNextWeather();

                OnWeatherStart?.Invoke(current_weather);

                yield return WaitForWeatherTransition(previous_weather, current_weather, current_settings.TransitionDuration);
            }
        }

        IEnumerator WaitForSkip(float duration)
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
            LerpWeather(from, to, t);
            yield return null;
        }

        LerpWeather(from, to, 1);
    }

    private WeatherInfo GetNextWeather()
    {
        var next = current_settings.Weathers
            .Where(x => x != current_weather) // Not the same as current weather
            .Where(x => current_weather != null && current_weather.Rain > 0.0f ? x.Rain < 0.01f : true) // No repeat rain
            .ToList()
            .Random();
        next ??= current_settings.Weathers.ToList().Random();
        return next;
    }

    public WeatherInfo GetCurrentWeather()
    {
        return current_weather;
    }

    private void LerpWeather(WeatherInfo from, WeatherInfo to, float t)
    {
        var env = GameScene.Instance.WorldEnvironment.Environment;
        var sun = GameScene.Instance.DirectionalLight;

        // Background
        env.BackgroundMode = to.BackgroundMode;
        env.BackgroundColor = from.BackgroundColor.Lerp(to.BackgroundColor, t);
        env.BackgroundEnergyMultiplier = Mathf.Lerp(from.BackgroundEnergyMultiplier, to.BackgroundEnergyMultiplier, t);

        // Ambient Color
        env.AmbientLightSource = to.AmbientSource;
        env.AmbientLightColor = from.AmbientColor.Lerp(to.AmbientColor, t);

        // Sun
        sun.LightColor = from.SunColor.Lerp(to.SunColor, Mathf.Clamp(t * 10, 0, 1));
        sun.ShadowEnabled = true;
        sun.ShadowOpacity = Mathf.Lerp(from.SunShadowOpacity, to.SunShadowOpacity, t);

        // Fog
        env.FogEnabled = FogLock.IsFree;
        env.FogDensity = Mathf.Lerp(from.IsNormalFog ? from.FogDensity : 0, to.IsNormalFog ? to.FogDensity : 0, t);
        env.FogLightColor = from.FogColor.Lerp(to.IsNormalFog ? to.FogColor : from.FogColor, t);
        env.FogAerialPerspective = Mathf.Lerp(from.IsNormalFog ? from.FogAerialPerspective : 1, to.IsNormalFog ? to.FogAerialPerspective : 1, t);

        // Volumetric
        env.VolumetricFogEnabled = FogLock.IsFree;
        env.VolumetricFogDensity = Mathf.Lerp(from.IsVolumetricFog ? from.FogDensity : 0, to.IsVolumetricFog ? to.FogDensity : 0, t);
        env.VolumetricFogAlbedo = from.FogColor.Lerp(to.IsVolumetricFog ? to.FogColor : from.FogColor, t);
        env.VolumetricFogEmission = from.FogColor.Lerp(to.IsVolumetricFog ? to.FogColor : from.FogColor, t);

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

    private void FogLock_Changed()
    {
        var env = GameScene.Instance.WorldEnvironment.Environment;
        env.FogEnabled = current_weather.IsNormalFog && FogLock.IsFree;
        env.VolumetricFogEnabled = current_weather.IsVolumetricFog && FogLock.IsFree;
    }

    public Color GetFogColor()
    {
        var env = GameScene.Instance.WorldEnvironment.Environment;
        if (env.FogEnabled)
        {
            return env.FogLightColor;
        }
        else
        {
            return env.VolumetricFogAlbedo;
        }
    }
}
