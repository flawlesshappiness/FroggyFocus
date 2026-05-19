using Godot;

public partial class GlitchScrollerScene : Scene
{
    [Export]
    public Node2D ScreenCover;

    [Export]
    public SideScrollerController Player;

    [Export]
    public TvScroller StartTv;

    [Export]
    public TvScroller EndTv;

    public override void _Ready()
    {
        base._Ready();
        WeatherController.Instance.StopWeather();
        RenderingServer.SetDefaultClearColor(Colors.Black);

        ScreenCover.Show();

        var tv = GameFlags.IsFlag(TvTravel.ScrollerSideFlag, 0) ? StartTv : EndTv;
        tv.SpawnPlayer(Player);
    }
}
