using Godot;
using System;
using System.Collections;
using System.Linq;

public partial class WindController : ResourceController<WindCollection, WindInfo>
{
    public static WindController Instance => Singleton.Get<WindController>();
    public override string Directory => "Wind";

    public Action<float> OnWindIntensityChanged;

    private WindInfo info;
    private AudioStreamPlayer asp_wind;
    private Vector2 intensity_range;
    private Coroutine cr_wind;
    private float current_volume;
    private float current_intensity;
    private double wind_time;
    private double wind_scale;

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
            Text = "Set wind intensity",
            Action = SelectIntensity
        });

        void SelectIntensity(DebugView v)
        {
            v.SetContent_Search();

            for (float i = 0; i <= 1.1f; i += 0.1f)
            {
                var level = i;
                v.ContentSearch.AddItem(i.ToString("0.0"), () => SetIntensityDebug(v, level));
            }

            v.ContentSearch.UpdateButtons();
        }

        void SetIntensityDebug(DebugView v, float value)
        {
            SetIntensity(value);
            v.Close();
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        var wind_time_mul = Mathf.Lerp(0.5, 20.0, current_intensity);
        wind_time += delta * wind_time_mul;
        RenderingServer.GlobalShaderParameterSet("global_wind_time", wind_time);
    }

    public void StartWind()
    {
        StopWind();

        info = GetInfo();
        asp_wind = info.SoundInfo.Play(new SoundOverride
        {
            Bus = AudioBusNames.Environment,
        });
        asp_wind.VolumeLinear = 0;

        cr_wind = this.StartCoroutine(Cr, "wind");
        IEnumerator Cr()
        {
            while (true)
            {
                var noise = (info.Noise.GetNoise1D(GameTime.Time) + 1) / 2;
                var intensity = Mathf.Lerp(intensity_range.X, intensity_range.Y, noise);
                SetIntensity(intensity);
                yield return null;
            }
        }
    }

    public void StopWind()
    {
        if (asp_wind != null)
        {
            SetIntensity(0);
            asp_wind?.QueueFree();
            asp_wind = null;
        }

        Coroutine.Stop(cr_wind);
    }

    private WindInfo GetInfo()
    {
        return Collection.Resources.ToList().Random();
    }

    public void SetIntensityRange(Vector2 range)
    {
        intensity_range = range;
    }

    public void SetIntensity(float t)
    {
        current_intensity = t;
        var target_volume = Mathf.Lerp(0.0f, 0.5f, t);
        current_volume = Mathf.Lerp(current_volume, target_volume, GameTime.DeltaTime);
        asp_wind.VolumeLinear = current_volume;
        OnWindIntensityChanged?.Invoke(t);

        wind_scale = Mathf.Lerp(0.5, 1.0, current_intensity);
        RenderingServer.GlobalShaderParameterSet("global_wind_scale", wind_scale);
    }
}
