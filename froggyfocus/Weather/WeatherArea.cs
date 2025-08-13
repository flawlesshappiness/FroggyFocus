using Godot;

public partial class WeatherArea : Area3D
{
    [Export]
    public WeatherInfo WeatherInfo;

    [Export]
    public float FadeDuration;

    public override void _Ready()
    {
        base._Ready();
        BodyEntered += PlayerEntered;
        BodyExited += PlayerExited;
    }

    private void PlayerEntered(GodotObject go)
    {
        WeatherController.Instance.SetNextWeather(WeatherInfo, FadeDuration);
    }

    private void PlayerExited(GodotObject go)
    {
        WeatherController.Instance.SetNextWeather(null, FadeDuration);
    }
}
