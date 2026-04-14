using Godot;
using System.Linq;
using System.Text.Json;

public partial class RaceGhostController : ResourceController<RaceGhostCollection, RaceGhostInfo>
{
    public static RaceGhostController Instance => Singleton.Get<RaceGhostController>();
    public override string Directory => "Race";

    public bool RecordGhostEnabled { get; private set; }
    public RaceGhostData CurrentData { get; private set; }

    private float GhostRecordStart { get; set; }
    private float GhostRecordNext { get; set; }
    private float GhostRecordTime => GameTime.Time - GhostRecordStart;
    private const float TICK_TIME = 0.05f;

    public override void _Ready()
    {
        base._Ready();
        RegisterDebugActions();
    }

    private void RegisterDebugActions()
    {
        var category = "RACE";

        Debug.RegisterAction(new DebugAction
        {
            Category = category,
            Text = "Ghost",
            Action = ListActions
        });

        void ListActions(DebugView v)
        {
            v.SetContent_Search();

            v.ContentSearch.AddItem($"Record ghost: {RecordGhostEnabled}", () => SetRecordGhost(v, !RecordGhostEnabled));

            if (CurrentData != null)
            {
                v.ContentSearch.AddItem($"Show ghost data", () => ShowGhostData(v));
                v.ContentSearch.AddItem($"Save ghost data", () => SaveGhostData(v));
            }

            v.ContentSearch.UpdateButtons();
        }

        void SetRecordGhost(DebugView v, bool enabled)
        {
            RecordGhostEnabled = enabled;
            ListActions(v);
        }

        void ShowGhostData(DebugView v)
        {
            v.SetContent_List();
            v.ContentList.Clear();

            foreach (var snapshot in CurrentData.Snapshots)
            {
                v.ContentList.AddText($"{snapshot.Time}: {snapshot.Position} {snapshot.Rotation} {snapshot.Animation}");
            }
        }

        void SaveGhostData(DebugView v)
        {
            v.PopupStringInput("Filename", "", s =>
            {
                SaveToFile(s);
                v.Close();
            });
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        Process_PlayerSnapshot();
    }

    private void Process_PlayerSnapshot()
    {
        if (!RecordGhostEnabled) return;
        if (CurrentData == null) return;
        if (GameTime.Time < GhostRecordNext) return;
        GhostRecordNext += TICK_TIME;

        var data = CreatePlayerSnapshot();
        CurrentData.Snapshots.Add(data);
    }

    public RaceGhostInfo GetInfo(string id)
    {
        return Collection.Resources.FirstOrDefault(x => x.Id == id);
    }

    public RaceGhostData GetData(string id)
    {
        var info = GetInfo(id);
        if (info == null) return null;

        if (FileAccess.FileExists(info.Path))
        {
            var json = FileAccess.GetFileAsString(info.Path);
            return JsonSerializer.Deserialize<RaceGhostData>(json);
        }

        return null;
    }

    public void StartRecordingGhost()
    {
        if (!RecordGhostEnabled) return;

        GhostRecordStart = GameTime.Time;
        CurrentData = new();
        Player.Instance.Character.OnAnimationStarted += Player_AnimationStarted;
    }

    public void EndRecordingGhost()
    {
        RecordGhostEnabled = false;
        Player.Instance.Character.OnAnimationStarted -= Player_AnimationStarted;
    }

    private void Player_AnimationStarted(string animation)
    {
        var data = CreatePlayerSnapshot();
        data.Animation = animation;
        CurrentData?.Snapshots.Add(data);
    }

    public RaceGhost CreateGhost()
    {
        var node = Collection.RaceGhostPrefab.Instantiate<RaceGhost>();
        node.SetParent(Scene.Current);
        return node;
    }

    private RaceGhostSnapshot CreatePlayerSnapshot()
    {
        var p = Player.Instance.GlobalPosition;
        var r = Player.Instance.Character.GlobalRotation;
        var data = new RaceGhostSnapshot
        {
            Time = GhostRecordTime,
            PositionX = p.X,
            PositionY = p.Y,
            PositionZ = p.Z,
            RotationX = r.X,
            RotationY = r.Y,
            RotationZ = r.Z,
        };
        return data;
    }

    private void SaveToFile(string filename)
    {
        var path = $"res://Race/Data/{filename}.txt";
        var json = JsonSerializer.Serialize(CurrentData, CurrentData.GetType(), new JsonSerializerOptions { WriteIndented = true, IncludeFields = true });
        using var file = FileAccess.Open(path, FileAccess.ModeFlags.Write);
        file.StoreLine(json);
    }
}
