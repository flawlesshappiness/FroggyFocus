using Godot;

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
        WeatherController.Instance.StartWeather(new Godot.Collections.Array<WeatherInfo> { Weather });
    }
}
