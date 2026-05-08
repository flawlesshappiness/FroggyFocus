using Godot;
using System.Collections.Generic;

public partial class ScreenshotSceneSetup : Node3D
{
    [Export]
    public string Id;

    [Export]
    public Camera3D Camera;

    [Export]
    public WeatherInfo Weather;

    protected virtual void OnShowSetup() { }

    public void ShowSetup()
    {
        OnShowSetup();
        Show();
        Camera.Current = true;

        WeatherController.Instance.StartWeather(new WeatherController.Settings
        {
            Weathers = new List<WeatherInfo> { Weather },
            InitialTransitionDuration = 0.01f,
            TransitionDuration = 0.01f,
            WeatherDuration = 999f,
        });
    }
}
