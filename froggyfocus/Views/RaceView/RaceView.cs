using Godot;

public partial class RaceView : View
{
    public static RaceView Instance => Get<RaceView>();

    [Export]
    public Label CheckpointLabel;

    [Export]
    public AudioStreamPlayer SfxCheckpoint;

    public override void _Ready()
    {
        base._Ready();
        RaceController.Instance.OnCheckpoint += Checkpoint;
    }

    protected override void OnShow()
    {
        base.OnShow();
        UpdateCheckpointLabel();
    }

    private void Checkpoint()
    {
        SfxCheckpoint.Play();
        UpdateCheckpointLabel();
    }

    public void UpdateCheckpointLabel()
    {
        var track = RaceController.Instance.CurrentTrack;
        var index = track?.CheckpointIndex ?? 0;
        var max = track?.CheckpointCount ?? 0;
        CheckpointLabel.Text = $"{index} / {max}";
    }
}
