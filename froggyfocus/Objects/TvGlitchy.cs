using Godot;
using System;

public partial class TvGlitchy : Area3D, IInteractable
{
    [Export]
    public HandInInfo HandInInfo;

    [Export]
    public AnimationPlayer AnimationPlayer;

    [Export]
    public CollisionShape3D Collider;

    [Export]
    public Node3D MatrixLabelParent;

    [Export]
    public PackedScene MatrixLabelPrefab;

    public bool IsCompleted => HandIn.GetOrCreateData(HandInInfo.Id).ClaimedCount > 0;

    private bool active_dialogue;

    public event Action OnCompleted;

    public override void _Ready()
    {
        base._Ready();
        InitializeMatrixLabels();
        InitializeHandIn();

        HandInController.Instance.OnHandInClaimed += HandInClaimed;
        DialogueController.Instance.OnNodeEnded += DialogueNodeEnded;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        HandInController.Instance.OnHandInClaimed -= HandInClaimed;
        DialogueController.Instance.OnNodeEnded -= DialogueNodeEnded;
    }

    private void InitializeHandIn()
    {
        HandIn.InitializeData(HandInInfo);
        SetCompleted(IsCompleted);
    }

    private void InitializeMatrixLabels()
    {
        var rng = new RandomNumberGenerator();
        var count = 5;
        var extent = 1.3f;
        var scale_range = new Vector2(0.002f, 0.005f);

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
        if (IsCompleted)
        {

        }
        else
        {
            active_dialogue = true;
            DialogueController.Instance.StartDialogue("##GLITCH_TV_REQUEST##");
        }
    }

    private void HandInClaimed(string id)
    {
        if (id != HandInInfo.Id) return;

        SetCompleted(true);
        OnCompleted?.Invoke();
    }

    private void DialogueNodeEnded(string id)
    {
        if (!active_dialogue) return;

        if (id == "##GLITCH_TV_REQUEST##")
        {
            var data = HandIn.GetOrCreateData(HandInInfo.Id);
            HandInView.Instance.ShowPopup(data);
        }

        active_dialogue = false;
    }

    public void DebugSetCompleted(bool completed)
    {
        var count = completed ? 1 : 0;
        var data = HandIn.GetOrCreateData(HandInInfo.Id);
        data.ClaimedCount = count;

        SetCompleted(completed);
    }

    private void SetCompleted(bool completed)
    {
        var anim = completed ? "completed" : "glitchy";
        AnimationPlayer.Play(anim);

        Collider.Disabled = completed;
    }
}
