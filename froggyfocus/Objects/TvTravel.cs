using Godot;
using Godot.Collections;
using System.Linq;

public partial class TvTravel : Area3D, IInteractable
{
    [Export]
    public string TravelScene;

    [Export]
    public string StartNode;

    [Export]
    public bool IsGoingUp;

    [Export]
    public AnimationPlayer AnimationPlayer;

    [Export]
    public Node3D MatrixLabelParent;

    [Export]
    public PackedScene MatrixLabelPrefab;

    [Export]
    public AudioStreamPlayer SfxComplete;

    [Export]
    public Array<TvGlitchy> Tvs;

    private string DebugId => nameof(TvTravel) + GetInstanceId();

    private bool is_on;
    private bool initialized;

    public override void _Ready()
    {
        base._Ready();

        RegisterDebugActions();
        InitializeMatrixLabels();

        foreach (var tv in Tvs)
        {
            tv.OnCompleted += TvChanged;
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (!initialized)
        {
            initialized = true;
            Initialize();
        }
    }

    private void RegisterDebugActions()
    {
        var category = "GLITCH TVs";

        if (!IsGoingUp)
        {
            Debug.RegisterAction(new DebugAction
            {
                Id = DebugId,
                Category = category,
                Text = "Open",
                Action = v => SetOpen(v, true)
            });

            Debug.RegisterAction(new DebugAction
            {
                Id = DebugId,
                Category = category,
                Text = "Close",
                Action = v => SetOpen(v, false)
            });
        }

        void SetOpen(DebugView v, bool open)
        {
            Tvs.ForEach(x => x.DebugSetCompleted(open));
            TvChanged();

            Data.Game.Save();
            v.Close();
        }
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        Debug.RemoveActions(DebugId);
    }

    private void InitializeMatrixLabels()
    {
        var rng = new RandomNumberGenerator();
        var count = 20;
        var extent = 2.5f;
        var scale_range = new Vector2(0.005f, 0.02f);

        for (int i = 0; i < count; i++)
        {
            var label = MatrixLabelPrefab.Instantiate<Node3D>();
            label.SetParent(MatrixLabelParent);

            var x = rng.RandfRange(-extent, extent);
            var z = rng.RandfRange(-extent, extent);
            label.Position = new Vector3(x, 0, z);

            label.Scale = Vector3.One * scale_range.Range(rng.Randf());
        }
    }

    private void Initialize()
    {
        var all_on = IsAllTVsOn();
        SetOn(all_on);
    }

    public void Interact()
    {
        if (is_on)
        {
            Travel();
        }
        else
        {
            DialogueController.Instance.StartDialogue("##TV_TRAVEL_OFF##");
        }
    }

    private void SetOn(bool on)
    {
        is_on = on;
        MatrixLabelParent.Visible = on;

        var anim = on ? "on" : "off";
        AnimationPlayer.Play(anim);

        MatrixLabelParent.Visible = on;
    }

    private bool IsAllTVsOn()
    {
        return Tvs.Count == 0 || Tvs.All(x => x.IsCompleted);
    }

    private void Travel()
    {
        Data.Game.CurrentScene = TravelScene;
        Data.Game.StartingNode = StartNode;
        Data.Game.Save();

        GlitchTransitionView.Instance.IsGoingUp = IsGoingUp;
        GlitchTransitionView.Instance.StartTransition();
    }

    private void TvChanged()
    {
        var all_on = IsAllTVsOn();
        SetOn(all_on || IsGoingUp);

        if (all_on)
        {
            SfxComplete.Play();
            DialogueController.Instance.StartDialogue("##GLITCH_TV_COMPLETE##");
        }
    }
}
