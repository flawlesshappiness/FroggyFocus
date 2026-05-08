using Godot;
using System.Collections.Generic;

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
        WeatherController.Instance.StartWeather(new WeatherController.Settings
        {
            Weathers = new List<WeatherInfo> { WeatherInfo },
            InitialTransitionDuration = FadeDuration,
            TransitionDuration = FadeDuration,
            WeatherDuration = 999f,
        });
    }

    private void PlayerExited(GodotObject go)
    {
        GameScene.Instance.StartWeather();
    }
}
