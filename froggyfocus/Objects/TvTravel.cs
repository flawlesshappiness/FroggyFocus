using Godot;
using Godot.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class TvTravel : Area3D, IInteractable
{
    [Export]
    public string TravelScene;

    [Export]
    public string StartNode;

    [Export]
    public AnimationPlayer AnimationPlayer;

    [Export]
    public Node3D MatrixLabelParent;

    [Export]
    public Array<TvGlitchy> Tvs;

    private bool is_on;
    private List<MatrixLabel> matrix_labels = new();

    public override void _Ready()
    {
        base._Ready();
        matrix_labels = MatrixLabelParent.GetNodesInChildren<MatrixLabel>();

        TvChanged();
        foreach (var tv in Tvs)
        {
            tv.OnTvChanged += TvChanged;
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
        matrix_labels.ForEach(x => x.SetOn(on));

        var anim = on ? "on" : "off";
        AnimationPlayer.Play(anim);
    }

    private bool IsAllTVsOn()
    {
        return Tvs.Count == 0 || Tvs.All(x => x.IsOn);
    }

    private void Travel()
    {
        Data.Game.CurrentScene = TravelScene;
        Data.Game.StartingNode = StartNode;
        Data.Game.Save();

        Scene.Goto(Data.Game.CurrentScene);
    }

    private void TvChanged()
    {
        var all_on = IsAllTVsOn();
        if (all_on && !is_on)
        {
            SetOn(true);
        }
        else if (is_on)
        {
            SetOn(false);
        }
    }
}
