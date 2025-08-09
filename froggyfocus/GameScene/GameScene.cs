using Godot;
using Godot.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class GameScene : Scene
{
    public static GameScene Instance { get; private set; }

    [Export]
    public DirectionalLight3D DirectionalLight;

    [Export]
    public WorldEnvironment WorldEnvironment;

    [Export]
    public Array<WeatherInfo> Weathers = new();

    [Export]
    public Array<FocusEvent> FocusEvents = new();

    private string current_focus_event_id;
    private List<FocusHotSpotArea> hotspot_areas;

    public override void _Ready()
    {
        base._Ready();
        Instance = this;

        FocusEventController.Instance.OnFocusEventCompleted += _ => FocusEventEnded();
        FocusEventController.Instance.OnFocusEventFailed += _ => FocusEventEnded();

        MusicController.Instance.StartMusic();
        WeatherController.Instance.StartWeather(Weathers);
        FocusHotSpotController.Instance.Start();

        HideFocusEvents();
    }

    protected override void Initialize()
    {
        base.Initialize();

        if (!string.IsNullOrEmpty(Data.Game.StartingNode))
        {
            var node = this.GetNodeInChildren<Node3D>(Data.Game.StartingNode);
            if (IsInstanceValid(node))
            {
                Player.Instance.GlobalPosition = node.GlobalPosition;
            }
        }
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

    public List<FocusHotSpotArea> GetFocusHotSpotAreas()
    {
        if (hotspot_areas == null)
        {
            hotspot_areas = this.GetNodesInChildren<FocusHotSpotArea>().ToList();
        }

        return hotspot_areas;
    }
}
