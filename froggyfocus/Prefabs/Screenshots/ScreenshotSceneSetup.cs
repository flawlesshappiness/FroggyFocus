using Godot;

public partial class ScreenshotSceneSetup : Node3D
{
    [Export]
    public Camera3D Camera;

    [Export]
    public WeatherInfo Weather;

    public void ShowSetup()
    {
        Show();
        Camera.Current = true;
        WeatherController.Instance.StartWeather(new Godot.Collections.Array<WeatherInfo> { Weather });
    }
}
