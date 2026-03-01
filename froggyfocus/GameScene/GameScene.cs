using FlawLizArt.FocusEvent;
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
    public FocusEvent FocusEvent { get; private set; }

    [Export]
    public Array<WeatherInfo> Weathers = new();

    private string CurrentFocusEventId => focus_event_ids.FirstOrDefault();

    private List<string> focus_event_ids = new();
    private List<FocusHotSpotArea> hotspot_areas;
    private List<WorldBug> world_bugs;

    public override void _Ready()
    {
        base._Ready();
        Instance = this;

        world_bugs = WorldBugParent?.GetNodesInChildren<WorldBug>() ?? new List<WorldBug>();

        MusicController.Instance.StartMusic();
        WeatherController.Instance.StartWeather(Weathers);
        FocusHotSpotController.Instance.Start();
        WorldBugController.Instance.Start();
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
                Player.Instance.SetRespawnPosition(node.GlobalPosition);
            }
        }
    }

    public void ClearFocusEventId(string id)
    {
        focus_event_ids.Remove(id);
    }

    public void SetFocusEventId(string id)
    {
        focus_event_ids.Add(id);
    }

    public void StartFocusEvent()
    {
        FocusEvent.StartEvent(new FocusEvent.Settings
        {
            Id = CurrentFocusEventId
        });
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
        if (!IsInstanceValid(Player.Instance)) return null;

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
        var id = CurrentFocusEventId;
        var info = FocusEventController.Instance.GetInfo(id);
        return info != null;
    }

    public bool HasFocusEvent()
    {
        return !string.IsNullOrEmpty(CurrentFocusEventId);
    }
}
