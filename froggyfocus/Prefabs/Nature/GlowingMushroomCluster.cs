using Godot;
using Godot.Collections;

public partial class GlowingMushroomCluster : Node3DScript
{
    [Export]
    public Array<GpuParticles3D> Particles;

    public override void _Ready()
    {
        base._Ready();
        WeatherController.Instance.OnWeatherStart += WeatherStart;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        WeatherController.Instance.OnWeatherStart -= WeatherStart;
    }

    protected override void Initialize()
    {
        base.Initialize();

        var weather = WeatherController.Instance.GetCurrentWeather();
        var glow = GlowingMushroom.ShouldGlow(weather);
        SetGlowing(glow);
    }

    private void WeatherStart(WeatherInfo info)
    {
        var glow = GlowingMushroom.ShouldGlow(info);
        Particles.ForEach(x => x.Emitting = glow);
    }

    private void SetGlowing(bool glow)
    {
        Particles.ForEach(x => x.Emitting = glow);
    }
}
