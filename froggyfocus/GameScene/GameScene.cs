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
    public FocusEvent FocusEvent;

    [Export]
    public AudioStreamPlayer BgmMain;

    [Export]
    public AudioStreamPlayer BgmFocus;

    [Export]
    public Array<WeatherInfo> Weathers = new();

    private string CurrentFocusEventId => focus_event_ids.FirstOrDefault();

    private List<string> focus_event_ids = new();
    private List<FocusHotSpotArea> hotspot_areas;
    private List<WorldBug> world_bugs = new();

    public override void _Ready()
    {
        base._Ready();
        Instance = this;

        world_bugs = this.GetNodesInChildren<WorldBug>() ?? new List<WorldBug>();

        WeatherController.Instance.StartWeather(Weathers);
        //FocusHotSpotController.Instance.Start();
        WorldBugController.Instance.Start();

        FocusEventController.Instance.OnFocusEventStarted += FocusEvent_Started;
        FocusEventController.Instance.OnFocusEventEnded += FocusEvent_Ended;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        FocusEventController.Instance.OnFocusEventStarted -= FocusEvent_Started;
        FocusEventController.Instance.OnFocusEventEnded -= FocusEvent_Ended;
    }

    protected override void Initialize()
    {
        base.Initialize();

        InitializeMusic();

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

    private void InitializeMusic()
    {
        if (BgmMain != null)
        {
            BgmMain.Play();
            BgmMain.FadeIn(0.5f, BgmMain.VolumeDb);
        }

        if (BgmFocus != null)
        {
            BgmFocus.Play();
            BgmFocus.VolumeLinear = 0f;
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

    private void FocusEvent_Started(FocusEvent focus_event)
    {
        if (BgmFocus != null)
        {
            BgmFocus.FadeIn(1f, 0f);
        }
    }

    private void FocusEvent_Ended(FocusEventResult result)
    {
        if (BgmFocus != null)
        {
            BgmFocus.FadeOut(1f);
        }
    }
}
