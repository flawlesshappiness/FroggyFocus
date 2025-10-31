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
    public Array<TvGlitchy> Tvs;

    private bool is_on;

    public override void _Ready()
    {
        base._Ready();

        InitializeMatrixLabels();

        TvChanged();
        foreach (var tv in Tvs)
        {
            tv.OnCompleted += TvChanged;
        }
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
        SetOn(all_on);
    }
}
