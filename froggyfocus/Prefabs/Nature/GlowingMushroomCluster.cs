using Godot;
using System.Collections;

public partial class GlowingMushroomCluster : Node3DScript
{
    [Export]
    public OmniLight3D Light;

    private float light_energy;

    public override void _Ready()
    {
        base._Ready();
        WeatherController.Instance.OnWeatherStart += WeatherStart;

        InitializeLight();
    }

    protected override void Initialize()
    {
        base.Initialize();

        var weather = WeatherController.Instance.GetCurrentWeather();
        var glow = GlowingMushroom.ShouldGlow(weather);
        SetGlowing(glow);
    }

    private void InitializeLight()
    {
        light_energy = Light.LightEnergy;
        Light.LightEnergy = 0;
    }

    private void WeatherStart(WeatherInfo info)
    {
        var glow = GlowingMushroom.ShouldGlow(info);

        this.StartCoroutine(Cr, "glow");
        IEnumerator Cr()
        {
            var e_light_start = Light.LightEnergy;
            var e_light_end = glow ? light_energy : 0;
            var duration = WeatherController.Instance.GetTransitionDuration();
            yield return LerpEnumerator.Lerp01(duration, f =>
            {
                Light.LightEnergy = Mathf.Lerp(e_light_start, e_light_end, f);
            });
        }
    }

    private void SetGlowing(bool glow)
    {
        Light.LightEnergy = glow ? light_energy : 0;
    }
}
