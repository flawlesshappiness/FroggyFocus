using Godot;
using Godot.Collections;
using System.Linq;

public partial class GameScene : Scene
{
    public static GameScene Instance { get; private set; }

    [Export]
    public DirectionalLight3D DirectionalLight;

    [Export]
    public WorldEnvironment WorldEnvironment;

    [Export]
    public Array<FocusEvent> FocusEvents = new();

    private string current_focus_event_id;

    public override void _Ready()
    {
        base._Ready();
        Instance = this;

        FocusEventController.Instance.OnFocusEventCompleted += _ => FocusEventEnded();
        FocusEventController.Instance.OnFocusEventFailed += _ => FocusEventEnded();

        MusicController.Instance.StartMusic();
        WeatherController.Instance.StartWeather();
        FocusHotSpotController.Instance.Start();

        HideFocusEvents();
    }

    public void SetFocusEventId(string id)
    {
        current_focus_event_id = id;
    }

    public void StartFocusEvent()
    {
        var focus_event = FocusEvents.FirstOrDefault(x => x.Id == current_focus_event_id) ?? FocusEvents.First();
        focus_event.StartEvent();
    }

    private void HideFocusEvents()
    {
        FocusEvents.ForEach(x => x.Hide());
    }

    private void FocusEventEnded()
    {
        HideFocusEvents();
    }
}
