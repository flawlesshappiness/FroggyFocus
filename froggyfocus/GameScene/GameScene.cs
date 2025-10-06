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
    public Node3D WorldBugParent;

    [Export]
    public Node3D FocusEventParent;

    [Export]
    public Array<WeatherInfo> Weathers = new();

    public List<FocusEvent> FocusEvents { get; private set; } = new();

    private string current_focus_event_id;
    private List<FocusHotSpotArea> hotspot_areas;
    private List<WorldBug> world_bugs;

    public override void _Ready()
    {
        base._Ready();
        Instance = this;

        FocusEventController.Instance.OnFocusEventCompleted += FocusEventEnded;
        FocusEventController.Instance.OnFocusEventFailed += FocusEventEnded;

        world_bugs = WorldBugParent?.GetNodesInChildren<WorldBug>() ?? new List<WorldBug>();
        FocusEvents = FocusEventParent?.GetNodesInChildren<FocusEvent>() ?? new List<FocusEvent>();

        MusicController.Instance.StartMusic();
        WeatherController.Instance.StartWeather(Weathers);
        FocusHotSpotController.Instance.Start();
        WorldBugController.Instance.Start();

        HideFocusEvents();
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        FocusEventController.Instance.OnFocusEventCompleted -= FocusEventEnded;
        FocusEventController.Instance.OnFocusEventFailed -= FocusEventEnded;
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

    public void ClearFocusEventId()
    {
        current_focus_event_id = string.Empty;
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

    private void FocusEventEnded(FocusEventResult result)
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

    public WorldBug GetClosestWorldBug()
    {
        var player_pos = Player.Instance.GlobalPosition;
        return world_bugs
            .Where(x => !x.IsRunning && x.GlobalPosition.DistanceTo(player_pos) > WorldBug.MIN_DIST_TO_PLAYER)
            .OrderBy(x => x.GlobalPosition.DistanceTo(player_pos))
            .Take(6)
            .ToList()
            .Random();
    }

    public bool HasFocusEventTargets()
    {
        return FocusEvents.Any(x => x.Info.Characters.Count > 0);
    }

    public bool HasFocusEvent()
    {
        var focus_event = FocusEvents.FirstOrDefault(x => x.Id == current_focus_event_id);
        return focus_event != null;
    }
}
