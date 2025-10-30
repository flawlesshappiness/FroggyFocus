using Godot;
using System;

public partial class TvGlitchy : Area3D, IInteractable
{
    [Export]
    public string GameFlagId;

    [Export]
    public AnimationPlayer AnimationPlayer;

    [Export]
    public AudioStreamPlayer3D SfxOn;

    [Export]
    public Node3D MatrixLabelParent;

    [Export]
    public PackedScene MatrixLabelPrefab;

    public bool IsOn => GameFlags.IsFlag(GameFlagId, 1);

    public event Action OnTvChanged;

    public override void _Ready()
    {
        base._Ready();

        InitializeMatrixLabels();

        if (IsOn)
        {
            SetOn(IsOn);
        }
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
        SetOn(!IsOn);
        Data.Game.Save();
        OnTvChanged?.Invoke();

        if (IsOn)
        {
            DialogueController.Instance.StartDialogue("##GLITCH_TV_ON##");
        }
    }

    private void SetOn(bool on)
    {
        GameFlags.SetFlag(GameFlagId, on ? 1 : 0);

        var anim = on ? "on" : "off";
        AnimationPlayer.Play(anim);

        MatrixLabelParent.Visible = on;

        SfxOn.Play();
    }
}
